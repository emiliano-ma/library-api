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

    [Fact]
    public async Task CreatingABook()
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
      // testBooks.Verify(set => set.Add(It.Is<Book>(e => e.Title == "Titile" && e.Author == "TestName" && e.Available == true)), Times.Once);
      testDbContext.Verify(ctx => ctx.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatingABookBadRequest()
    {
      // Arrange
      var controller = new BooksController(testDbContext.Object);

      var newBook = new Book
      {
        
        Available = true
      };

      // Act
      var result = await controller.PostBook(newBook);
      // controller.ModelState.AddModelError("error", "some error");

      // Assert
      testDbContext.Verify(ctx => ctx.SaveChangesAsync(), Times.Once);
      // Assert.IsType<BadRequestObjectResult>(result);
    }


    // [Fact]
    public async Task GetSingleBook()
    {
      // Arrange
      int bookId = 123;

      var controller = new BooksController(testDbContext.Object);
      
      // Act
      var result = await controller.GetBook(bookId);

      // Assert
      var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
      Assert.Equal(bookId, notFoundObjectResult.Value);
    }

  }
}
