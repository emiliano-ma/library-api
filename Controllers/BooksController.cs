using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;

namespace LibraryApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BooksController : ControllerBase
  {
    private readonly LibraryContext _context;

    public BooksController(LibraryContext context)
    {
      _context = context;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] string Title, [FromQuery] string Author)
    {
      if (!string.IsNullOrEmpty(Title))
        return await _context.Books
              .Where(b => b.Title == Title)
              .ToListAsync();

      else if (!string.IsNullOrEmpty(Author))
        return await _context.Books
              .Where(b => b.Author == Author)
              .ToListAsync();

      else
        return await _context.Books.ToListAsync();
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
      var book = await _context.Books.FindAsync(id);

      if (book == null)
      {
        return NotFound();
      }

      return book;
    }


    // PATCH: api/Books/5
    [HttpPatch("{id}")]
    public async Task<ActionResult<Book>> PatchBook(int id, Book book)
    {
      var bookPatch = await _context.Books.FindAsync(id);

      if (bookPatch == null)
      {
        return NotFound();
      }

      bookPatch.ReaderId = book.ReaderId;
      bookPatch.Available = book.Available;
      bookPatch.UpdatedAt = DateTime.Now;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException exception)
      {
        throw new Exception($"Something went wrong: {exception}");
      }

      return bookPatch;
    }

     // PATCH: api/Books/save
    [HttpPatch("save")]
    public async Task<IActionResult> SaveBook(Book book)
    {
      if (!BookExists(book.BookId))
      {
        _context.Books.Add(book);
        try
        {
          await _context.SaveChangesAsync();
          return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
        }
        catch (Exception exception)
        {
          throw new Exception($"Something went wrong: {exception}");
        }
      }
      else
      {
        _context.Entry(book).State = EntityState.Modified;
        try
        {
          await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
          throw new Exception($"Something went wrong: {exception}");
        }
        return Content($"The book has been updated with '{book.Title}' '{book.Author}' '{book.Available}'", "text/ plain");
      }
    }

    // PUT: api/Books/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
      if (id != book.BookId)
      {
        return BadRequest();
      }

      _context.Entry(book).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!BookExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Books
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
      _context.Books.Add(book);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Book>> DeleteBook(int id)
    {
      var book = await _context.Books.FindAsync(id);
      if (book == null)
      {
        return NotFound();
      }

      _context.Books.Remove(book);
      await _context.SaveChangesAsync();

      return book;
    }

    private bool BookExists(int id)
    {
      return _context.Books.Any(e => e.BookId == id);
    }
  }
}
