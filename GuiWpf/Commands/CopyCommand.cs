using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GuiWpf.Commands
{
    public class CopyCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var text = parameter as string;
            if (text != null)
            {
                Clipboard.SetText(text);
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
