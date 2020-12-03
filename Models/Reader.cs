using System.Collections.Generic;

namespace LibraryApi.Models
{
  public class Reader
  {
    public int ReaderId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Book> Books { get; }
  }
}