using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
  public class Reader
  {
    public int ReaderId { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    public List<Book> Books { get; }
  }
}