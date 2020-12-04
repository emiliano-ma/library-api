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
  public class ReadersController : ControllerBase
  {
    private readonly LibraryContext _context;

    public ReadersController(LibraryContext context)
    {
      _context = context;
    }

    // GET: api/Readers
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
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReader(int id, Reader reader)
    {
      if (id != reader.ReaderId)
      {
        return BadRequest();
      }

      _context.Entry(reader).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ReaderExists(id))
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

    // POST: api/Readers
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    public async Task<ActionResult<Reader>> PostReader(Reader reader)
    {
      _context.Readers.Add(reader);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetReader), new { id = reader.ReaderId }, reader);
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
      await _context.SaveChangesAsync();

      return reader;
    }

    private bool ReaderExists(int id)
    {
      return _context.Readers.Any(e => e.ReaderId == id);
    }
  }
}
