using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;

namespace LibraryApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReadersController : ControllerBase
  {
    private readonly ILibraryContext _context;

    public ReadersController(ILibraryContext context)
    {
      _context = context;
    }

    // GET: api/Readers - api/Readers?email={user@mail.com} - api/Readers?name={name}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reader>>> GetReaders([FromQuery] string Name, [FromQuery] string Email)
    {
      if (!string.IsNullOrEmpty(Name))
        return await _context.Readers
              .Where(b => b.Name == Name)
              .ToListAsync();

      else if (!string.IsNullOrEmpty(Email))
        return await _context.Readers
              .Where(b => b.Email == Email)
              .ToListAsync();

      else
        return await _context.Readers
            .Include(reader => reader.Books)
            .ToListAsync();
    }

    // GET: api/Readers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Reader>> GetReader(int id)
    {
      var reader = await _context.Readers.FindAsync(id);

      if (reader == null)
      {
        return NotFound();
      }

      return reader;
    }

    // PUT: api/Readers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReader(int id, Reader reader)
    {
      if (id != reader.ReaderId)
      {
        return BadRequest();
      }

      var readerUpdate = await _context.Readers.FindAsync(id);

      if (readerUpdate == null)
      {
        return NotFound();
      }
      readerUpdate.Name = reader.Name;
      readerUpdate.Email = reader.Email;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException exception)
      {
        if (!ReaderExists(id))
        {
          return NotFound();
        }
        else
        {
          throw new Exception($"Something went wrong: {exception}");
        }
      }

      return Content($"The Reader has been updated with Name:'{reader.Name}', email:'{reader.Email}'", "text/ plain");
    }

    // POST: api/Readers
    [HttpPost]
    public async Task<ActionResult<Reader>> PostReader(Reader reader)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      _context.Readers.Add(reader);

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (Exception exception)
      {
        throw new Exception($"Something went wrong: {exception}");
      }
      return CreatedAtAction(nameof(GetReader), new { id = reader.ReaderId }, reader);
    }

    // PATCH: api/Readers/save
    [HttpPatch("save")]
    public async Task<IActionResult> SaveReader(Reader reader)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      // if reader id update reader else create reader
      if (!ReaderExists(reader.ReaderId))
      {
        _context.Readers.Add(reader);
        try
        {
          await _context.SaveChangesAsync();
          return CreatedAtAction(nameof(GetReader), new { id = reader.ReaderId }, reader);
        }
        catch (Exception exception)
        {
          throw new Exception($"Something went wrong: {exception}");
        }
      }
      else
      {
        var readerUpdate = await _context.Readers.FindAsync(reader.ReaderId);

        if (readerUpdate == null)
        {
          return NotFound();
        }
        readerUpdate.Name = reader.Name;
        readerUpdate.Email = reader.Email;
        try
        {
          await _context.SaveChangesAsync();
          return Content($"The Reader has been updated with Name:'{reader.Name}', email:'{reader.Email}'", "text/ plain");
        }
        catch (Exception exception)
        {
          throw new Exception($"Something went wrong: {exception}");
        }
      }
    }

    // DELETE: api/Readers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Reader>> DeleteReader(int id)
    {
      var reader = await _context.Readers.FindAsync(id);
      if (reader == null)
      {
        return NotFound();
      }

      _context.Readers.Remove(reader);

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (Exception exception)
      {
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
