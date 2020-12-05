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

    // GET: api/Books - api/books?author={author} - api/books?title={title}
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

    // PUT: api/Books/5
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
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
      _context.Books.Add(book);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
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
          return Content($"The book has been updated with Title:'{book.Title}', Author:'{book.Author}', Available: '{book.Available}'", "text/ plain");
        }
        catch (Exception exception)
        {
          throw new Exception($"Something went wrong: {exception}");
        }
      }
    }

    // PATCH: api/Books/save/batch
    [HttpPatch("save/batch")]
    public async Task<IActionResult> SaveBooksBatch(IEnumerable<Book> books)
    {
      try
      {

        {
          foreach (var book in books)
          {

            if (!TitleExists(book.Title))
            {
              _context.Books.Add(book);
            }
            else
            {
              var bookUpdate = await _context.Books
                                .FirstOrDefaultAsync(e => e.Title == book.Title);

              if (bookUpdate == null)
              {
                return NotFound();
              }
              bookUpdate.Title = book.Title;
              bookUpdate.Author = book.Author;
              bookUpdate.Available = book.Available;
              bookUpdate.ReaderId = book.ReaderId;
              bookUpdate.UpdatedAt = DateTime.Now;
            }
          }
          await _context.SaveChangesAsync();
          return Content($"Batch succesffully handled", "text/ plain");
        }
      }
      catch (Exception exception)
      {
        throw new Exception($"Something went wrong: {exception}");
      }
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
    private bool TitleExists(string title)
    {
      return _context.Books.Any(e => e.Title == title);
    }
  }
}
