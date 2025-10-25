using LibraryManager.Models;
namespace LibraryManager.Services
{
    public interface IBookRepository
    {
        // CRUD Operations
        List<Book> GetAllBooks();
        Book GetBookById(int id);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(int id);
        void ToggleAvailability(int id);

        // Library-specific functions
        List<Book> GetAvailableBooks();
        List<Book> GetBorrowedBooks();

    }
}
