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

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly BookContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(BookContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            try
            {
                return await _context.Books.ToListAsync();
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
                var book = await _context.Books.FindAsync(id);

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
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

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

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

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
            return _context.Books.Any(e => e.Id == id);
        }

        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByPageAsync(int page = 1, int pageSize = 5)
        {
            try
            {
                return await _context.Books
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
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
                return await _context.Books
                    .Where(b => b.Author == author)
                    .ToListAsync();
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

        // Create a API to return the list of books based on the category
        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategoryAsync(string category)
        {
            try
            {
                return await _context.Books
                    .Where(b => b.Category == category)
                    .ToListAsync();
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