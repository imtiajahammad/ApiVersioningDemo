## ApiVersioningDemo

Here I will implement API versioning in ASP.NET Core 6 step by step.

### What and Why
- API evolves over time
- We do need to implement breaking changes sometimes  

This API changes should not affect our existing API consumers  

API versioning is the practice of smoothly managing changes to an API without breaking the client applications that consume the API. Versioning allows clients to continue using the existing REST API and only migrate their applications to the newer API versions when they are ready.

### When to Version API
- Removing or renaming an allowed parameter, request field or a response field
- Removing or renaming an endpoint
- Adding a required field or making a field required on the request
- Changing the type of request field or response field
- Changing the existing permission definitions
- Adding new validations

### Different Ways of Versioning API
- Query Parameter Versioning
- URI Versioning
- Custom HTTP Header Versioning
- Content Negotiation Versioning





1. Open Terminal and Go to your preferred directory and make a folder for your project solution
    ```
    mkdir ApiVersioningDemo
    cd ApiVersioningDemo
    ```
2. Create a blank solution with name **ApiVersioningDemo**
    ```
    dotnet new sln -n ApiVersioningDemo
    ```
3. Create a webapi project in the solution
    ```
    dotnet new webapi -f net6.0 -n ApiVersioningDemo
    ```
4. Add the project into the solution 
    ```
    dotnet sln add ApiVersioningDemo/ApiVersioningDemo.csproj
    ```
5. Create a gitignore file in the solution
    ```
    dotnet new gitignore
    ```
6. Create a readme.md file 
    ```
    touch README.md 
    ```
7. Now open the solution in visual studio code
    ```
    code .
    ```
8. Install the following packages
    ```
    dotnet add package Microsoft.AspNetCore.Mvc.Versioning
    ```
9. Create a class called Data and add the following code
    ```
    mkdir Models
    cd Models
    dotnet new class -n Data
    ```
    ```
    public class Data
    {
        public static readonly List<Book> Books = new List<Book>()
        {
            new Book()
            {
                Id = 1,
                Title = "Concurrency in C# Cookbook",
                Author = "Stephen Cleary"
            },
            new Book()
            {
                Id = 2,
                Title = "Designing Data-Intensive Applications",
                Author = "Martin Kleppmann"
            }
        };
    }
    ```
    ```
    dotnet new class -n Book
    ```
    ```
    public class Book
    {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    }
    ```
    ```
    cd Controllers
    dotnet new class -n BooksController
    ```
    ```
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = Data.Books;
            return Ok(books);
        }
    }
    ```
#### Query Parameter Versioning-start
10. If we run the project, we will get error. As we enabled the versioning but did not pass any version. For this scenario we can use default version without setting any version. specify api-version=1.0 as a query param 
    ```
    https://localhost:7070/api/Books?api-version=1.0
    ```
11. We can resolve the same scenario, by setting the default version=1 if there is no version specified in URI. Lets go to program.cs and add the versioning
    ```
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    });
    ```
Now the following url will work without the version specified
    ```
    https://localhost:7070/api/Books
    ```
#### Query Parameter Versioning-end
#### Add Multiple Versions for Single Endpoint-start
12. We can add multiple versions to the same controller and use **[MapToApiVersion]** attribute to map Actions to the different versions of endpoints like this
    ```
    cd Controllers
    dotnet new class -n MultipleBooksController
    ```
    ```
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public class MultipleBooksController : ControllerBase
    {
    [MapToApiVersion("1.0")]
    [HttpGet]
    public IActionResult GetBooks()
    {
        var books = Data.Books;
        return Ok(books);
    }
    [MapToApiVersion("2.0")]
    [HttpGet]
    public IActionResult GetBooksV2()
    {
        var books = Data.Books.Select(x => x.Title);
        return Ok(books);
    }    
    }
    ```
Now all these urls are going to work
    ```
    https://localhost:7076/api/MultipleBooksMultiple
    https://localhost:7076/api/MultipleBooksMultiple?api-version=1.0
    https://localhost:7076/api/MultipleBooksMultiple?api-version=2.0
    ```
13. We can also create separate controllers for individual versions.  

Let’s first create some folders to organize the controllers of different API versions better. We’ll create two folders called “v1” and “v2” inside the “Controllers” folder. Then we’ll move the BooksController.cs to the “v1” folder and will add .v1 to the namespace
    ```
    cd Controllers
    mkdir v1
    cd v1
    dotnet new class -n SeparateFolderBooksSeparateFolderController
    ```
    ```
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SeparateFolderBooksSeparateFolderController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = Data.Books;
            return Ok(books);
        }
    }
    ```
    ```
    cd Controllers
    mkdir v2
    cd v2
    dotnet new class -n SeparateFolderBooksSeparateFolderController
    ```
    ```
    [Route("api/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class SeparateFolderBooksSeparateFolderController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBooksV2()
        {
            var books = Data.Books.Select(x => x.Title);
            return Ok(books);
        }
    }
    ```
Now, if we send a get request to followings, everything should work
    ```
    https://localhost:7070/api/SeparateFolderBooksSeparateFolder?api-version=1.0
    https://localhost:7070/api/SeparateFolderBooksSeparateFolder?api-version=2.0
    ```
14. We should let our consumers know we’re supporting multiple versions. We can do this by adding ReportApiVersions=true inside AddApiVersioning like this
    ```
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
    });
    ```
Check the response headers for followings
    ```
    https://localhost:7070/api/SeparateFolderBooksSeparateFolder?api-version=1.0
    https://localhost:7070/api/SeparateFolderBooksSeparateFolder?api-version=2.0
    ```
#### Add Multiple Versions for Single Endpoint-end
#### URI Versioning-start
15. URI versioning is the most common and cleaner method. We can easily read which API version are we targeting right from the URI.
    ```
    https://localhost:7070/api/1.0/UriVersioningBooksUriVersioning
    https://localhost:7070/api/2.0/UriVersioningBooksUriVersioning
    ```
    ```
    cd Controllers
    cd v1
    dotnet new class -n UriVersioningBooksUriVersioningController
    ```
    ```
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class UriVersioningBooksUriVersioningController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = Data.Books;
            return Ok(books);
        }
    }
    ```
    ```
    cd Controllers
    cd v2
    dotnet new class -n UriVersioningBooksUriVersioningController
    ```
    ```
    [ApiVersion("2.0")]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    public class UriVersioningBooksUriVersioningController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBooksV2()
        {
            var books = Data.Books.Select(x => x.Title);
            return Ok(books);
        }
    }
    ```
#### URI Versioning-end
#### Custom HTTP Header Versioning-start
16. If we don’t want to change the URI of the API, we can send the version in the HTTP Header. To enable this, we have to modify our configuration
    ```
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
    });
    ```
    ```
    cd Controllers
    cd v1
    dotnet new class -n CustomHttpHeaderBooksCustomHttpHeaderController
    ```
    ```
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomHttpHeaderBooksCustomHttpHeaderController : ControllerBase
    {
    [HttpGet]
    public IActionResult GetBooks()
    {
        var books = Data.Books.Select(x => x.Title);
        return Ok(books);
    }
    }
    ```
    ```
    cd Controllers
    cd v2
    dotnet new class -n CustomHttpHeaderBooksCustomHttpHeaderController
    ```
    ```
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomHttpHeaderBooksCustomHttpHeaderController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBooksV2()
        {
            var books = Data.Books.Select(x => x.Title);
            return Ok(books);
        }
    }
    ```
Make a postman call with the custom request header **x-api-version**
    ```
    https://localhost:7070/api/CustomHttpHeaderBooksCustomHttpHeader
    ```
If we use query string versioning, by default ASP.NET core accepts api-version as query parameter if specified. If we want to support different a parameter name, we can use a QueryStringApiVersionReader class instead
    ```
    options.ApiVersionReader = new QueryStringApiVersionReader("x-api-version");
    ```
    ```
    https://localhost:7070/api/CustomHttpHeaderBooksCustomHttpHeader?x-api-version=2.0
    ```
#### Custom HTTP Header Versioning-end
#### Content Negotiation Versioning (Media Versioning)-start
17. Similar to custom header versioning, we don’t need to modify the URI in this approach. We only change the Accept header values. In this case, the scheme preserves our URIs between versions.
    ```
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        //options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        options.ApiVersionReader = new MediaTypeApiVersionReader("version");
    });
    ```
In the request header, add **Accept** for **application/json;version=2** value and check it on Postman  
    ```
    https://localhost:7070/api/CustomHttpHeaderBooksCustomHttpHeader
    ```
#### Content Negotiation Versioning (Media Versioning)-start
#### Combining Multiple Approaches-start
18. We’re not bound to use only one approach of versioning. We can give consumers multiple ways to choose. ApiVersionReader has a Combine method that we can use to specify multiple readers. Let’s say we want to support the query parameter versioning, the accept header, and the custom header versioning. We can update the versioning service as follows:
    ```
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new MediaTypeApiVersionReader("version"),
            new HeaderApiVersionReader("x-api-version"),
            new QueryStringApiVersionReader("x-api-version")
        );
    });
    ```
#### Combining Multiple Approaches-end
#### Deprecating Versions-start
19. If we want to deprecate an API version without deleting it, we can use the Deprecated property as follows:
    ```
    [ApiVersion("2.0", Deprecated = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {}
    ```
#### Deprecating Versions-end
#### Using Conventions-start
20. Instead of adding the [ApiVersion] attribute to the controllers, we can assign these versions to different controllers in the configuration instead.
    ```
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        options.Conventions.Controller<VersioningAPI.Controllers.v1.BooksController>()
            .HasApiVersion(new ApiVersion(1, 0));
        options.Conventions.Controller<VersioningAPI.Controllers.v2.BooksController>()
            .HasDeprecatedApiVersion(new ApiVersion(2, 0));

    });
    ```

#### Using Conventions-end






https://vivasoftltd.com/api-versioning-in-asp-net-core/







9. Configure API versioning options into Program.cs file
    ```
    builder.Services.AddApiVersioning(options =>
    {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    });
    ```
10. 


#### Reference: https://www.c-sharpcorner.com/article/api-versioning-in-net-6/
