using LibraryManager.ViewModels;
using System.Windows;

namespace LibraryManager.Views
{
    /// <summary>
    /// Interaction logic for AddBookView.xaml
    /// </summary>
    public partial class AddBookView : Window
    {
        public AddBookView()
        {
            InitializeComponent();
            DataContext = new AddBookViewModel(this);
        }
    }
}
