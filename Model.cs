using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi
{
  public class LibraryContext : DbContext
  {
    // public LibraryContext(DbContextOptions<LibraryContext> options)
    //       : base(options)
    // {
    // }
    public DbSet<Reader> Readers { get; set; }
    public DbSet<Book> Books { get; set; }

  }

  public class Reader
  {
    public int ReaderId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Book> Books { get; } = new List<Book>();
  }

  public class Book
  {
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool Available { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public Book()
       {          
         this.CreatedDate  = DateTime.Today;
         this.ModifiedDate = DateTime.Today;
       }
    public int ReaderId { get; set; }
    public Reader Reader { get; set; }
  }
}