using NUnit.Framework;
using Moq;
using BookApi.Controllers;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Repositories;
using Microsoft.Extensions.Logging;

namespace BookApi.Tests
{
    public class BooksControllerTests
    {
        private Mock<IBookRepository> _mockRepository;
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

            _mockRepository = new Mock<IBookRepository>();
            _mockRepository.Setup(repo => repo.GetBooksAsync()).ReturnsAsync(_books);
            _mockRepository.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => _books.FirstOrDefault(b => b.Id == id));
            _mockRepository.Setup(repo => repo.CreateBookAsync(It.IsAny<Book>())).Callback<Book>(b => _books.Add(b)).Returns(Task.CompletedTask);
            _mockRepository.Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(repo => repo.DeleteBookAsync(It.IsAny<Book>())).Callback<Book>(b => _books.Remove(b)).Returns(Task.CompletedTask);
            _mockRepository.Setup(repo => repo.GetBooksByPageAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((int page, int pageSize) => _books.Skip((page - 1) * pageSize).Take(pageSize).ToList());
            _mockRepository.Setup(repo => repo.GetBooksByAuthorAsync(It.IsAny<string>())).ReturnsAsync((string author) => _books.Where(b => b.Author == author).ToList());
            _mockRepository.Setup(repo => repo.BookExists(It.IsAny<int>())).Returns((int id) => _books.Any(b => b.Id == id));

            _controller = new BooksController(_mockRepository.Object, Mock.Of<ILogger<BooksController>>());
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
            _mockRepository.Verify(m => m.CreateBookAsync(It.IsAny<Book>()), Times.Once());
        }

        [Test]
        public async Task UpdateBook_UpdatesBook()
        {
            var updatedBook = new Book { Id = 1, Title = "Updated Book 1", Author = "Updated Author 1" };
            var result = await _controller.UpdateBook(1, updatedBook);
            var noContentResult = result as NoContentResult;

            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
            _mockRepository.Verify(m => m.UpdateBookAsync(It.IsAny<Book>()), Times.Once());
        }

        // [Test]
        // public async Task UpdateBook_ReturnsBadRequest()
        // {
        //     var updatedBook = new Book { Id = 2, Title = "Updated Book 2", Author = "Updated Author 2" };
        //     var result = await _controller.UpdateBook(1, updatedBook);

        //     Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        // }

        // [Test]
        // public async Task DeleteBook_DeletesBook()
        // {
        //     var result = await _controller.DeleteBook(1);

        //     Assert.IsInstanceOf<NoContentResult>(result);
        //     _mockRepository.Verify(m => m.DeleteBookAsync(It.IsAny<Book>()), Times.Once());
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

        [Test]
        public async Task GetBooksByCategoryAsync_ReturnsBooksByCategory()
        {
            var result = await _controller.GetBooksByCategoryAsync("Category 1");
            var okResult = result.Result as OkObjectResult;
            var books = okResult.Value as List<Book>;

            Assert.That(books.Count, Is.EqualTo(1));
        }
    }
}