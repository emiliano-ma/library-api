using Microsoft.EntityFrameworkCore;
using LibraryApi.Models;
using System.Threading.Tasks;
using System;

namespace LibraryApi.Data
{
  public interface ILibraryContext : IDisposable
  {
    public DbSet<Reader> Readers { get; set; }
    public DbSet<Book> Books { get; set; }
    Task<int> SaveChangesAsync();
  }
  public class LibraryContext : DbContext, ILibraryContext
  {
    public LibraryContext(DbContextOptions<LibraryContext> options)
          : base(options)
    {
    }
    public virtual DbSet<Reader> Readers { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Book>().ToTable("Book");
      modelBuilder.Entity<Reader>().ToTable("Reader");
    }
  }
}