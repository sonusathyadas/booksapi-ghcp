using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BookApi.Controllers;
using BookApi.Data;
using BookApi.Models;

namespace BookApi.Tests
{
    public class BooksControllerTests
    {
        private readonly BooksController _controller;
        private readonly BookContext _context;

        public BooksControllerTests()
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new BookContext(options);
            _controller = new BooksController(_context);
        }

        [Fact]
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Arrange
            var book1 = new Book { Title = "Book 1", Author = "Author 1", Language = "English", Category = "Fiction" };
            var book2 = new Book { Title = "Book 2", Author = "Author 2", Language = "English", Category = "Non-Fiction" };
            _context.Books.Add(book1);
            _context.Books.Add(book2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task GetBookById_ReturnsBook()
        {
            // Arrange
            var book = new Book { Title = "Book 1", Author = "Author 1", Language = "English", Category = "Fiction" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBookById(book.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(book.Title, returnedBook.Title);
        }

        [Fact]
        public async Task CreateBook_ReturnsCreatedBook()
        {
            // Arrange
            var book = new Book { Title = "New Book", Author = "New Author", Language = "English", Category = "Fiction" };

            // Act
            var result = await _controller.CreateBook(book);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedBook = Assert.IsType<Book>(createdResult.Value);
            Assert.Equal(book.Title, returnedBook.Title);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNoContent()
        {
            // Arrange
            var book = new Book { Title = "Book 1", Author = "Author 1", Language = "English", Category = "Fiction" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            book.Title = "Updated Title";

            // Act
            var result = await _controller.UpdateBook(book.Id, book);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent()
        {
            // Arrange
            var book = new Book { Title = "Book 1", Author = "Author 1", Language = "English", Category = "Fiction" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteBook(book.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}