using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
/*
namespace ApiVersioningDemo.Controllers;

[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
public class MultipleBooksMultipleController : ControllerBase
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
}*/