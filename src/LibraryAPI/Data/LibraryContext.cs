using Microsoft.EntityFrameworkCore;
using LibraryApi.Models;

namespace LibraryApi.Data
{
  public class LibraryContext : DbContext
  {
    public LibraryContext(DbContextOptions<LibraryContext> options)
          : base(options)
    {
    }
    public DbSet<Reader> Readers { get; set; }
    public DbSet<Book> Books { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<Reader>().ToTable("Reader");
        }
  }
}