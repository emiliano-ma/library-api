using System;
using System.ComponentModel.DataAnnotations;


namespace LibraryApi.Models
{
  public class Book
  {
    public int BookId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Author name is required")]
    public string Author { get; set; }
    [Required(ErrorMessage = "Availability must be specified")]
    public bool Available { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Book()
    {
      this.UpdatedAt = DateTime.Now;
    }
    public int? ReaderId { get; set; }
  }
}
