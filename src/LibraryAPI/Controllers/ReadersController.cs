using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;
using NLog;

namespace LibraryApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReadersController : ControllerBase
  {
    private static Logger logger = LogManager.GetLogger("libraryLoggerRules");

    private readonly ILibraryContext _context;

    public ReadersController(ILibraryContext context)
    {
      _context = context;
    }

    // GET: api/Readers - api/Readers?email={user@mail.com} - api/Readers?name={name}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reader>>> GetReaders([FromQuery] string Name, [FromQuery] string Email)
    {
      logger.Info("Starting to process GET request api/Readers...");

      if (!string.IsNullOrEmpty(Name))
      {
        logger.Info($"Requesting list of Readers with Name: '{Name}'.");
        return await _context.Readers
              .Where(b => b.Name == Name)
              .ToListAsync();
      }
      else if (!string.IsNullOrEmpty(Email))
      {
        logger.Info($"Requesting list of Readers with Email: '{Email}'.");

        return await _context.Readers
              .Where(b => b.Email == Email)
              .ToListAsync();
      }
      else
        logger.Info("No query params were found, requesting full-list of Readers.");

      return await _context.Readers
          .Include(reader => reader.Books)
          .ToListAsync();
    }

    // GET: api/Readers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Reader>> GetReader(int id)
    {
      logger.Info($"Starting to process GET request api/Readers/{id} ...");

      var reader = await _context.Readers.FindAsync(id);

      if (reader == null)
      {
        logger.Error($"The {id} didn't match an existing record");
        return NotFound();
      }
      logger.Info("Request successfully handled, exiting controller.");
      return reader;
    }

    // PUT: api/Readers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReader(int id, Reader reader)
    {
      logger.Info($"Starting to process PUT request api/Readers/{id} ...");

      if (id != reader.ReaderId)
      {
        logger.Error("Bad Request.");
        return BadRequest();
      }
      logger.Info($"Trying to find the reader with id: {id}");
      var readerUpdate = await _context.Readers.FindAsync(id);

      if (readerUpdate == null)
      {
        logger.Error("The record could not be loaded.");
        return NotFound();
      }
      readerUpdate.Name = reader.Name;
      readerUpdate.Email = reader.Email;

      try
      {
        await _context.SaveChangesAsync();
        logger.Info($"The reader with id:{id} was updated. Exiting controller.");

      }
      catch (DbUpdateConcurrencyException exception)
      {
        if (!ReaderExists(id))
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

      return Content($"The Reader has been updated with Name:'{reader.Name}', email:'{reader.Email}'", "text/ plain");
    }

    // POST: api/Readers
    [HttpPost]
    public async Task<ActionResult<Reader>> PostReader(Reader reader)
    {
      logger.Info($"Starting to process POST request api/Readers...");
      if (!ModelState.IsValid)
      {
        logger.Error("Bad Request.");
        return BadRequest(ModelState);
      }

      _context.Readers.Add(reader);

      try
      {
        await _context.SaveChangesAsync();
        logger.Info($"The reader '{reader.Name}' was saved. Exiting controller.");
      }
      catch (Exception exception)
      {
        logger.Error($"An exception ocurred: {exception}");
        throw new Exception($"Something went wrong: {exception}");
      }
      return CreatedAtAction(nameof(GetReader), new { id = reader.ReaderId }, reader);
    }

    // PATCH: api/Readers/save
    [HttpPatch("save")]
    public async Task<IActionResult> SaveReader(Reader reader)
    {
      logger.Info($"Starting to process PATCH request api/Readers/save...");
      if (!ModelState.IsValid)
      {
        logger.Error("Bad Request.");
        return BadRequest(ModelState);
      }
      // if reader id update reader else create reader
      if (!ReaderExists(reader.ReaderId))
      {
        logger.Info($"The reader '{reader.Name}' doesn't exist. Creating new reader.");
        _context.Readers.Add(reader);
        try
        {
          await _context.SaveChangesAsync();
          logger.Info($"The reader was saved. Exiting controller...");
          return CreatedAtAction(nameof(GetReader), new { id = reader.ReaderId }, reader);
        }
        catch (Exception exception)
        {
          logger.Error($"An exception ocurred: {exception}");
          throw new Exception($"Something went wrong: {exception}");
        }
      }
      else
      {
        logger.Info($"Trying to update the reader with id: {reader.ReaderId}.");
        var readerUpdate = await _context.Readers.FindAsync(reader.ReaderId);

        if (readerUpdate == null)
        {
          logger.Error("The record could not be loaded.");
          return NotFound();
        }
        readerUpdate.Name = reader.Name;
        readerUpdate.Email = reader.Email;
        try
        {
          await _context.SaveChangesAsync();
          logger.Info($"The reader with id:{reader.ReaderId} was updated. Exiting controller.");
          return Content($"The Reader has been updated with Name:'{reader.Name}', email:'{reader.Email}'", "text/ plain");
        }
        catch (Exception exception)
        {
          logger.Error($"An exception ocurred: {exception}");
          throw new Exception($"Something went wrong: {exception}");
        }
      }
    }

    // DELETE: api/Readers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Reader>> DeleteReader(int id)
    {
      logger.Info($"Starting to process DELETE request api/Readers/{id}...");
      var reader = await _context.Readers.FindAsync(id);
      if (reader == null)
      {
        logger.Error($"The {id} didn't match an existing record");
        return NotFound();
      }

      _context.Readers.Remove(reader);

      try
      {
        await _context.SaveChangesAsync();
        logger.Info($"The reader with id:{reader.ReaderId} was deleted. Exiting controller.");
      }
      catch (Exception exception)
      {
        logger.Error($"An exception ocurred: {exception}");
        throw new Exception($"Something went wrong: {exception}");
      }

      return Content($"Reader '{reader.Name}' succesffully deleted", "text/ plain");
    }

    private bool ReaderExists(int id)
    {
      return _context.Readers.Any(e => e.ReaderId == id);
    }
  }
}
