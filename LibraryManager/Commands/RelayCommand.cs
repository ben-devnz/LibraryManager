using System.Windows.Input;

namespace LibraryManager.Commands
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action<object> _Execute { get; set; }
        private Predicate<object> _CanExecute { get; set; }

        public RelayCommand(Action<object> executeMethod, Predicate<object> CanExecuteMethod)
        {
            _Execute = executeMethod;
            _CanExecute = CanExecuteMethod;
        }

        public bool CanExecute(object? parameter)
        {
            return _CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _Execute(parameter);
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
