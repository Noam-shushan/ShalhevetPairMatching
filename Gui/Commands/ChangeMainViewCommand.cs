using PairMatching.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PairMatching.Gui.Commands
{
    public class ChangeMainViewCommand : ICommand
    {
        public event Action<NavigationCurrentView?> ChangeMainView;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var view = parameter as NavigationCurrentView?;
            if (view != null && ChangeMainView != null)
            {
                ChangeMainView(view);
            }
        }
    }
}
