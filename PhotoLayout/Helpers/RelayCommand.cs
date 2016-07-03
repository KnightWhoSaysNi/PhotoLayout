using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoLayout.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        // TODO Check if this works as it should
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
