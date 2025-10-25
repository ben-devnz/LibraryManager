using LibraryManager.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LibraryManager.ViewModels
{
    public class AddBookViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // ==================== FIELDS ====================

        private string _title;
        private string _author;
        private string _isbn;
        private int _yearPublished;
        private Window _window;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
                (SaveBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged();
                (SaveBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        public string ISBN
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                OnPropertyChanged();
                (SaveBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        public int YearPublished
        {
            get { return _yearPublished; }
            set
            {
                _yearPublished = value;
                OnPropertyChanged();
                (SaveBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // ==================== COMMANDS ====================

        public ICommand SaveBookCommand { get; set; }
        public ICommand CancelBookCommand { get; set; }


        // ==================== CONSTRUCTOR ====================

        public AddBookViewModel(Window window)
        {
            _window = window;
            _title = string.Empty;
            _author = string.Empty;
            _isbn = string.Empty;
            _yearPublished = 0;

            // Init Commands
            SaveBookCommand = new RelayCommand
            (
                executeMethod: (param) => SaveBook(),
                CanExecuteMethod: (param) => AllFieldsComplete() // Can only execute if All fields are complete
            );
            CancelBookCommand = new RelayCommand
            (
                executeMethod: (param) => Cancel(),
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

        private void SaveBook()
        {
            if (!AllFieldsComplete())
            {
                MessageBox.Show
                    (
                    "Please fill in all fields before saving.",
                    "Incomplete Form",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
                return;
            }

            _window.DialogResult = true;
            _window.Close();
        }
        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();

        }
    }
}
