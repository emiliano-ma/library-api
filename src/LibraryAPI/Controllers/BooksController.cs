using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;
using NLog;

namespace LibraryApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BooksController : ControllerBase
  {
    private static Logger logger = LogManager.GetLogger("libraryLoggerRules");
    private readonly ILibraryContext _context;

    public BooksController(ILibraryContext context)
    {
      _context = context;
    }

    // GET: api/Books - api/books?author={author} - api/books?title={title}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] string Title, [FromQuery] string Author)
    {
      logger.Info("Starting to process GET request api/Books...");
      if (!string.IsNullOrEmpty(Title))
      {
        logger.Info($"Requesting list of books with title: '{Title}'.");
        return await _context.Books
              .Where(b => b.Title == Title)
              .ToListAsync();
      }
      else if (!string.IsNullOrEmpty(Author))
      {
        logger.Info($"Requesting list of books with Author name: '{Author}'.");
        return await _context.Books
              .Where(b => b.Author == Author)
              .ToListAsync();
      }
      else
        logger.Info("No query params were found, requesting full-list of books.");
      return await _context.Books.ToListAsync();
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
      logger.Info($"Starting to process GET request api/Books/{id} ...");
      var book = await _context.Books.FindAsync(id);

      if (book == null)
      {
        logger.Error($"The {id} didn't match an existing record");
        return NotFound();
      }
      logger.Info("Request successfully handled, exiting controller.");
      return book;
    }

    // PUT: api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
      logger.Info($"Starting to process PUT request api/Books/{id} ...");
      if (id != book.BookId)
      {
        logger.Error("Bad Request.");
        return BadRequest();
      }
      logger.Info($"Trying to find the book with id: {id}");
      var bookUpdate = await _context.Books.FindAsync(id);

      if (bookUpdate == null)
      {
        logger.Error("The record could not be loaded.");
        return NotFound();
      }
      bookUpdate.Title = book.Title;
      bookUpdate.Author = book.Author;
      bookUpdate.Available = book.Available;
      bookUpdate.ReaderId = book.ReaderId;
      bookUpdate.UpdatedAt = DateTime.Now;

      try
      {
        await _context.SaveChangesAsync();
        logger.Info($"The book with id:{id} was updated. Exiting controller.");
      }
      catch (DbUpdateConcurrencyException exception)
      {
        if (!BookExists(id))
        {
          logger.Error($"The {id} didn't match an existing record");
          return NotFound();
        }
        else
        {
          logger.Error($"An exception ocurred");
          throw new Exception($"Something went wrong: {exception}");
        }
      }

      return Content($"The book has been updated with Title:'{book.Title}', Author:'{book.Author}', Available: '{book.Available}'", "text/ plain");
    }

    // POST: api/Books
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
      logger.Info($"Starting to process POST request api/Books...");
      if (!ModelState.IsValid)
      {
        logger.Error("Bad Request.");
        return BadRequest(ModelState);
      }

      _context.Books.Add(book);

      try
      {
        await _context.SaveChangesAsync();
        logger.Info($"The book '{book.Title}' was saved. Exiting controller.");
      }
      catch (Exception exception)
      {
        logger.Error($"An exception ocurred: {exception}");
        throw new Exception($"Something went wrong: {exception}");
      }
      return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
    }

    // PATCH: api/Books/save
    [HttpPatch("save")]
    public async Task<IActionResult> SaveBook(Book book)
    {
      logger.Info($"Starting to process PATCH request api/Books/save...");
      if (!ModelState.IsValid)
      {
        logger.Error("Bad Request.");
        return BadRequest(ModelState);
      }
      // if book id update book else create book
      if (!BookExists(book.BookId))
      {
        logger.Info($"The book with title: {book.Title} doesn't exist. Creating new book.");
        _context.Books.Add(book);
        try
        {
          await _context.SaveChangesAsync();
          logger.Info($"The book was saved. Exiting controller...");
          return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
        }
        catch (Exception exception)
        {
          logger.Error($"An exception ocurred: {exception}");
          throw new Exception($"Something went wrong: {exception}");
        }
      }
      else
      {
        logger.Info($"Trying to update the book with id: {book.BookId}.");
        var bookUpdate = await _context.Books.FindAsync(book.BookId);

        if (bookUpdate == null)
        {
          logger.Error("The record could not be loaded.");
          return NotFound();
        }
        bookUpdate.Title = book.Title;
        bookUpdate.Author = book.Author;
        bookUpdate.Available = book.Available;
        bookUpdate.ReaderId = book.ReaderId;
        bookUpdate.UpdatedAt = DateTime.Now;
        try
        {
          await _context.SaveChangesAsync();
          logger.Info($"The book with id:{book.BookId} was updated. Exiting controller.");
          return Content($"The book has been updated with Title:'{book.Title}', Author:'{book.Author}', Available: '{book.Available}'", "text/ plain");
        }
        catch (Exception exception)
        {
          logger.Error($"An exception ocurred: {exception}");
          throw new Exception($"Something went wrong: {exception}");
        }
      }
    }

    // PATCH: api/Books/save/batch
    [HttpPatch("save/batch")]
    public async Task<IActionResult> SaveBooksBatch(IEnumerable<Book> books)
    {
      logger.Info($"Starting to process PATCH request api/Books/save/batch...");
      // Takes a list of books and foreach -> 
      //      if title matches existing book update book else create book
      try
      {
        {
          foreach (var book in books)
          {

            if (!TitleExists(book.Title))
            {
              _context.Books.Add(book);
              logger.Info($"The book with Title {book.Title} doesn't exist. Creating new book.");
            }
            else
            {
              logger.Info($"Trying to update the book with Title {book.Title}.");
              var bookUpdate = await _context.Books
                                .FirstOrDefaultAsync(e => e.Title == book.Title);

              if (bookUpdate == null)
              {
                logger.Error("The record could not be loaded.");
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
          logger.Info($"Batch succesffully handled. Exiting controller.");
          return Content("Batch succesffully handled", "text/ plain");
        }
      }
      catch (Exception exception)
      {
        logger.Error($"An exception ocurred: {exception}");
        throw new Exception($"Something went wrong: {exception}");
      }
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Book>> DeleteBook(int id)
    {
      logger.Info($"Starting to process DELETE request api/Books/{id}...");
      var book = await _context.Books.FindAsync(id);
      if (book == null)
      {
        logger.Error($"The {id} didn't match an existing record");
        return NotFound();
      }

      _context.Books.Remove(book);

      try
      {
        await _context.SaveChangesAsync();
        logger.Info($"The book with id:{book.BookId} was deleted. Exiting controller.");
      }
      catch (Exception exception)
      {
        logger.Error($"An exception ocurred: {exception}");
        throw new Exception($"Something went wrong: {exception}");
      }

      return Content($"Book '{book.Title}' succesffully deleted", "text/ plain");
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
