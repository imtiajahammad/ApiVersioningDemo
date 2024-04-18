namespace ApiVersioningDemo;

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
