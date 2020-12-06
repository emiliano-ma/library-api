using Xunit;
using LibraryApi.Controllers;
using LibraryApi.Data;
using LibraryApi.Models;
using Moq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Tests.UnitTests
{
  public class BooksControllerTests
  {
    private readonly Mock<ILibraryContext> testDbContext;
    private readonly Mock<DbSet<Book>> testBooks;

    public BooksControllerTests()
    {
      testDbContext = new Mock<ILibraryContext>();

      testBooks = new Mock<DbSet<Book>>();

      testDbContext.Setup(ctx => ctx.Books).Returns(testBooks.Object);
    }

    // [WIP]

    [Fact]
    public async Task UpdatingABook()
    {
      // Arrange
      var controller = new BooksController(testDbContext.Object);

      var newBook = new Book
      {
        Title = "Title",
        Author = "TestName",
        Available = true
      };

      // Act
      var result = await controller.PostBook(newBook);

      // Assert
      testDbContext.Verify(ctx => ctx.SaveChangesAsync(), Times.Once);
    }
  }
}
