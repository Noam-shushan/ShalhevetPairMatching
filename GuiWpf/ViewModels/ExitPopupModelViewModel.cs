using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.ViewModels
{
    public class ExitPopupModelViewModel : ViewModelBase
    {
        private IPopupViewModle _dialogView;

        public ExitPopupModelViewModel(IPopupViewModle dialogView)
        {
            _dialogView = dialogView;
        }

        DelegateCommand _ExitCommand;
        public DelegateCommand ExitCommand => _ExitCommand ??= new(
        () =>
        {
            _dialogView.CloseDialog();
        });
    }
}
