using BookApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookApi.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task CreateBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        bool BookExists(int id);
        Task<IEnumerable<Book>> GetBooksByPageAsync(int page, int pageSize);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
    }
}
