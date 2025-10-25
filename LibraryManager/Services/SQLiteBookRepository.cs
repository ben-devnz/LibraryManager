using LibraryManager.Models;
using Microsoft.Data.Sqlite;

namespace LibraryManager.Services
{
    public class SQLiteBookRepository : IBookRepository
    {
        // ==================== FIELDS ====================

        private readonly string _connectionString;

        // ==================== CONSTRUCTOR ====================

        public SQLiteBookRepository(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
            SeedDataIfEmpty();
        }
        private void SeedDataIfEmpty()
        {
            var existingBooks = GetAllBooks();
            if (existingBooks.Count == 0)
            {
                AddBook(new Book
                {
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "978-0451524935",
                    YearPublished = 1949,
                    IsAvailable = true
                });

                AddBook(new Book
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "978-0061120084",
                    YearPublished = 1960,
                    IsAvailable = true
                });

                AddBook(new Book
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "978-0743273565",
                    YearPublished = 1925,
                    IsAvailable = false,
                    BorrowedDate = DateTime.Now.AddDays(-5)
                });

                AddBook(new Book
                {
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ISBN = "978-0141439518",
                    YearPublished = 1813,
                    IsAvailable = true
                });
            }
        }
        private void InitializeDatabase()
        {
            using var connection = OpenConnection();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    ISBN TEXT,
                    YearPublished INTEGER,
                    IsAvailable INTEGER NOT NULL,
                    BorrowedDate TEXT
            )";
            command.ExecuteNonQuery();
        }

        // ==================== PUBLIC METHODS (IMPLEMENTED INTERFACE) ====================

        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();

            using var connection = OpenConnection();


            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Books";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(BuildBookFromReader(reader));
            }
            return books;
        }

        public Book GetBookById(int id)
        {
            Book book = null;

            using var connection = OpenConnection();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Books WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                book = BuildBookFromReader(reader);
            }
            return book;
        }


        public void AddBook(Book book)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Books (Title, Author, ISBN, YearPublished, IsAvailable, BorrowedDate)
                VALUES ($title, $author, $isbn, $yearPublished, $isAvailable, $borrowedDate)
            ";
            command.Parameters.AddWithValue("$title", book.Title);
            command.Parameters.AddWithValue("$author", book.Author);
            command.Parameters.AddWithValue("$isbn", book.ISBN);
            command.Parameters.AddWithValue("$yearPublished", book.YearPublished);

            command.Parameters.AddWithValue("$isAvailable", book.IsAvailable ? 1 : 0);

            if (book.BorrowedDate.HasValue)
            {
                command.Parameters.AddWithValue("$borrowedDate", book.BorrowedDate.Value.ToString("o"));
            }
            else
            {
                command.Parameters.AddWithValue("$borrowedDate", DBNull.Value);
            }

            command.ExecuteNonQuery();
        }

        public void UpdateBook(Book book)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Books
                SET Title = $title,
                    Author = $author,
                    ISBN = $isbn,
                    YearPublished = $yearPublished,
                    IsAvailable = $isAvailable,
                    BorrowedDate = $borrowedDate
                WHERE Id = $id
            ";

            command.Parameters.AddWithValue("$title", book.Title);
            command.Parameters.AddWithValue("$author", book.Author);
            command.Parameters.AddWithValue("$isbn", book.ISBN);
            command.Parameters.AddWithValue("$yearPublished", book.YearPublished);

            command.Parameters.AddWithValue("$isAvailable", book.IsAvailable ? 1 : 0);


            if (book.BorrowedDate.HasValue)
            {
                command.Parameters.AddWithValue("$borrowedDate", book.BorrowedDate.Value.ToString("o"));
            }
            else
            {
                command.Parameters.AddWithValue("$borrowedDate", DBNull.Value);
            }

            command.Parameters.AddWithValue("$id", book.Id);

            command.ExecuteNonQuery();

        }

        public void DeleteBook(int id)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();

            command.CommandText = "DELETE FROM Books WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            command.ExecuteNonQuery();
        }

        public List<Book> GetAvailableBooks()
        {
            var books = new List<Book>();
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Books WHERE IsAvailable = 1";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(BuildBookFromReader(reader));
            }
            return books;
        }

        public List<Book> GetBorrowedBooks()
        {
            var books = new List<Book>();
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Books WHERE IsAvailable = 0";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(BuildBookFromReader(reader));
            }
            return books;
        }

        public void ToggleAvailability(int id)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Books
                SET 
                    IsAvailable = CASE IsAvailable WHEN 1 THEN 0 ELSE 1 END,
                    BorrowedDate = CASE IsAvailable
                                        WHEN 1 THEN strftime('%Y-%m-%dT%H:%M:%fZ', 'now') -- was available, now borrowing
                                        ELSE NULL
                                    END
                WHERE Id = $id;
            ";

            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();

        }

        // ==================== PRIVATE HELPER FUNCTIONS ====================

        private SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private Book BuildBookFromReader(SqliteDataReader reader)
        {
            return new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                ISBN = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                YearPublished = reader.GetInt32(4),
                IsAvailable = reader.GetInt32(5) == 1,
                BorrowedDate = reader.IsDBNull(6) ? null : DateTime.Parse(reader.GetString(6))
            };
        }
    }
}
