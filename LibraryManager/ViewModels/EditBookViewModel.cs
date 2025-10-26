using LibraryManager.Commands;
using LibraryManager.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LibraryManager.ViewModels
{
    public class EditBookViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // ==================== FIELDS ====================

        private Book _originalBook; // Reference to original
        private Window _window;

        private string _title;
        private string _author;
        private string _isbn;
        private int _yearPublished;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
                (SaveEditedBookCommand as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged();
                (SaveEditedBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string ISBN
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                OnPropertyChanged();
                (SaveEditedBookCommand as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        public int YearPublished
        {
            get { return _yearPublished; }
            set
            {
                _yearPublished = value;
                OnPropertyChanged();
                (SaveEditedBookCommand as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        // ==================== COMMANDS ====================
        public ICommand SaveEditedBookCommand { get; set; }
        public ICommand CancelEditedBookCommand { get; set; }

        // ==================== CONSTRUCTOR ====================
        public EditBookViewModel(Window window, Book bookToEdit)
        {
            _window = window;
            _originalBook = bookToEdit;

            // Pre populate with existing values
            _title = bookToEdit.Title;
            _author = bookToEdit.Author;
            _isbn = bookToEdit.ISBN;
            _yearPublished = bookToEdit.YearPublished;

            // Initialize Commands
            SaveEditedBookCommand = new RelayCommand(
                executeMethod: (param) => SaveEditedBook(),
                CanExecuteMethod: (param) => AllFieldsComplete()
            );
            CancelEditedBookCommand = new RelayCommand(
                executeMethod: (param) => CancelEditedBook(),
                CanExecuteMethod: (param) => true
            );
        }

        // ==================== PRIVATE METHODS ====================

        private bool AllFieldsComplete()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Author)
                && !string.IsNullOrWhiteSpace(ISBN)
                && YearPublished > 0;
        }

        private void SaveEditedBook()
        {
            if (!AllFieldsComplete()) return;

            _originalBook.Title = Title;
            _originalBook.Author = Author;
            _originalBook.ISBN = ISBN;
            _originalBook.YearPublished = YearPublished;

            _window.DialogResult = true;
            _window.Close();
        }

        private void CancelEditedBook()
        {
            _window.DialogResult = false;
            _window.Close();
        }


    }


}
