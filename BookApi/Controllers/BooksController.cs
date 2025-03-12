using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApi.Data;
using BookApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using BookApi.Repositories;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            try
            {
                return Ok(await _bookRepository.GetBooksAsync());
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error fetching books");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching books");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                return book;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database update error fetching book with id {id}");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching book with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _bookRepository.CreateBookAsync(book);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error creating book");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating book");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _bookRepository.UpdateBookAsync(book);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating book with id {id}", id);
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database update error updating book with id {id}");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error updating book with id {id}");
                return StatusCode(500, "Internal server error");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                await _bookRepository.DeleteBookAsync(book);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database update error deleting book with id {id}");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error deleting book with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool BookExists(int id)
        {
            return _bookRepository.BookExists(id);
        }

        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByPageAsync(int page = 1, int pageSize = 5)
        {
            try
            {
                return Ok(await _bookRepository.GetBooksByPageAsync(page, pageSize));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error fetching books by page");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching books by page");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("author")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthorAsync(string author)
        {
            try
            {
                return Ok(await _bookRepository.GetBooksByAuthorAsync(author));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database update error fetching books by author {author}");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching books by author {author}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategoryAsync(string category)
        {
            try
            {
                return Ok(await _bookRepository.GetBooksByCategoryAsync(category));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database update error fetching books by category {category}");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching books by category {category}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}