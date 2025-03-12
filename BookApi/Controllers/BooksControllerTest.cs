using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using BookApi.Controllers;
using BookApi.Data;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi.Tests
{
    public class BooksControllerTests
    {
        private Mock<BookContext> _mockContext;
        private Mock<DbSet<Book>> _mockSet;
        private BooksController _controller;
        private List<Book> _books;

        [SetUp]
        public void Setup()
        {
            _books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1" },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2" }
            };

            _mockSet = new Mock<DbSet<Book>>();
            _mockSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(_books.AsQueryable().Provider);
            _mockSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(_books.AsQueryable().Expression);
            _mockSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(_books.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(_books.AsQueryable().GetEnumerator());

            _mockContext = new Mock<BookContext>();
            _mockContext.Setup(c => c.Books).Returns(_mockSet.Object);

            _controller = new BooksController(_mockContext.Object);
        }

        [Test]
        public async Task GetBooks_ReturnsAllBooks()
        {
            var result = await _controller.GetBooks();
            var okResult = result.Result as OkObjectResult;
            var books = okResult.Value as List<Book>;

            Assert.That(books.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetBookById_ReturnsBook()
        {
            var result = await _controller.GetBookById(1);
            var okResult = result.Result as OkObjectResult;
            var book = okResult.Value as Book;

            Assert.That(book.Id, Is.EqualTo(1));
        }

        // [Test]
        // public async Task GetBookById_ReturnsNotFound()
        // {
        //     var result = await _controller.GetBookById(3);
        //     Assert.IsInstanceOf<NotFoundResult>(result.Result);
        // }

        [Test]
        public async Task CreateBook_AddsBook()
        {
            var newBook = new Book { Id = 3, Title = "Book 3", Author = "Author 3" };
            var result = await _controller.CreateBook(newBook);
            var createdAtActionResult = result.Result as CreatedAtActionResult;

            Assert.That(createdAtActionResult, Is.Not.Null);
            Assert.That(createdAtActionResult.StatusCode, Is.EqualTo(201));
            var book = createdAtActionResult.Value as Book;

            Assert.That(book.Id, Is.EqualTo(3));
            _mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task UpdateBook_UpdatesBook()
        {
            var updatedBook = new Book { Id = 1, Title = "Updated Book 1", Author = "Updated Author 1" };
            var result = await _controller.UpdateBook(1, updatedBook);
            var noContentResult = result as NoContentResult;

            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        // [Test]
        // public async Task UpdateBook_ReturnsBadRequest()
        // {
        //     var updatedBook = new Book { Id = 2, Title = "Updated Book 2", Author = "Updated Author 2" };
        //     var result = await _controller.UpdateBook(1, updatedBook);

        //     Assert.IsInstanceOf<BadRequestResult>(result);
        // }

        // [Test]
        // public async Task DeleteBook_DeletesBook()
        // {
        //     var result = await _controller.DeleteBook(1);

        //     Assert.IsInstanceOf<NoContentResult>(result);
        //     _mockSet.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once());
        //     _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        // }

        // [Test]
        // public async Task DeleteBook_ReturnsNotFound()
        // {
        //     var result = await _controller.DeleteBook(3);

        //     Assert.IsInstanceOf<NotFoundResult>(result);
        // }

        [Test]
        public async Task GetBooksByPageAsync_ReturnsPagedBooks()
        {
            var result = await _controller.GetBooksByPageAsync(1, 1);
            var okResult = result.Result as OkObjectResult;
            var books = okResult.Value as List<Book>;

            Assert.That(books.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetBooksByAuthorAsync_ReturnsBooksByAuthor()
        {
            var result = await _controller.GetBooksByAuthorAsync("Author 1");
            var okResult = result.Result as OkObjectResult;
            var books = okResult.Value as List<Book>;

            Assert.That(books.Count, Is.EqualTo(1));
        }
    }
}