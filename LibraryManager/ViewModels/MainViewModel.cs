using LibraryManager.Commands;
using LibraryManager.Models;
using LibraryManager.Services;
using LibraryManager.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LibraryManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // ==================== FIELDS ====================
        private IBookRepository _repository;
        private Book _selectedBook;

        // ==================== PROPERTIES ====================
        public ObservableCollection<Book> Books { get; set; }
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged();

                // Tell commands to re-check their canExecute
                (RemoveBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ToggleCheckOutCommand as RelayCommand)?.RaiseCanExecuteChanged();



            }
        }

        // ==================== COMMANDS ====================
        public ICommand AddBookCommand { get; set; }
        public ICommand RemoveBookCommand { get; set; }
        public ICommand UpdateBookCommand { get; set; }
        public ICommand GetAvailableBooksCommand { get; }
        public ICommand GetBorrowedBooksCommand { get; set; }
        public ICommand ShowAllBooksCommand { get; set; }
        public ICommand ToggleCheckOutCommand { get; set; }


        // ==================== CONSTRUCTOR ====================
        public MainViewModel()
        {
            // Set up database path
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbFolder = Path.Combine(appDataFolder, "LibraryManager");
            var dbPath = Path.Combine(dbFolder, "library.db");

            // Create Folder if it doesn't exist
            Directory.CreateDirectory(dbFolder);

            // Create repository and load data
            _repository = new SQLiteBookRepository(dbPath);

            // Initialize Books collection
            Books = new ObservableCollection<Book>();
            LoadBooks();

            // Initialize commands
            AddBookCommand = new RelayCommand
            (
                executeMethod: (param) => AddBook(),
                CanExecuteMethod: (param) => true
            );
            RemoveBookCommand = new RelayCommand
            (
                executeMethod: (param) => RemoveBook(),
                CanExecuteMethod: (param) => SelectedBook != null
            );
            UpdateBookCommand = new RelayCommand
            (
                executeMethod: (param) => UpdateBook(),
                CanExecuteMethod: (param) => SelectedBook != null
            );
            GetAvailableBooksCommand = new RelayCommand
            (
                executeMethod: (param) => GetAvailableBooks(),
                CanExecuteMethod: (param) => true
            );
            GetBorrowedBooksCommand = new RelayCommand
            (
                executeMethod: (param) => GetBorrowedBooks(),
                CanExecuteMethod: (param) => true
            );
            ShowAllBooksCommand = new RelayCommand
            (
                executeMethod: (param) => LoadBooks(),
                CanExecuteMethod: (param) => true
            );
            ToggleCheckOutCommand = new RelayCommand
            (
                executeMethod: (param) => ToggleAvailability(),
                CanExecuteMethod: (param) => SelectedBook != null
            );

        }

        private void LoadBooks()
        {
            Books.Clear();
            var booksFromDb = _repository.GetAllBooks();
            foreach (var book in booksFromDb)
            {
                Books.Add(book);
            }
        }

        private void AddBook()
        {
            // Create and show the AddBook Window
            var addWindow = new AddBookView();

            // Set the owner of the window
            addWindow.Owner = Application.Current.MainWindow;

            // Show as dialog (blocks until closed)
            bool? result = addWindow.ShowDialog();

            // If the user clicked save
            if (result == true)
            {
                // Get the ViewModel from the window
                var vm = addWindow.DataContext as AddBookViewModel;

                var newBook = new Book
                {
                    Title = vm.Title,
                    Author = vm.Author,
                    ISBN = vm.ISBN,
                    YearPublished = vm.YearPublished,
                    IsAvailable = true,
                    BorrowedDate = null
                };

                // Add book to repository
                _repository.AddBook(newBook);

                // Refresh the UI collection
                LoadBooks();
            }
        }

        private void RemoveBook()
        {
            if (SelectedBook != null)
            {
                _repository.DeleteBook(SelectedBook.Id);
                LoadBooks();
            }
        }
        private void UpdateBook()
        {
            if (SelectedBook != null)
            {
                _repository.UpdateBook(SelectedBook);
                LoadBooks();
            }
        }
        private void GetAvailableBooks()
        {
            Books.Clear();
            var availableBooks = _repository.GetAvailableBooks();

            foreach (var book in availableBooks)
            {
                Books.Add(book);
            }
        }
        private void GetBorrowedBooks()
        {
            Books.Clear();
            var availableBooks = _repository.GetBorrowedBooks();

            foreach (var book in availableBooks)
            {
                Books.Add(book);
            }
        }
        private void ToggleAvailability()
        {
            if (SelectedBook != null)
            {
                _repository.ToggleAvailability(SelectedBook.Id);
                LoadBooks();
            }
        }

    }
}
