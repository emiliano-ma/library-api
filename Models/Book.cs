using System;


namespace LibraryApi.Models
{
  public class Book
  {
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool Available { get; set; }
    public DateTime ModifiedDate { get; set; }
    public Book()
    {
      this.ModifiedDate = DateTime.Now;
    }
    public int ReaderId { get; set; }
  }
}
