using GuiWpf.Events;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.ViewModels
{
    public class DialogViewModel : ViewModelBase
    {
        readonly IEventAggregator _ea;

        public DialogViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }

        private bool _isModelOpen;
        public bool IsModelOpen
        {
            get => _isModelOpen;
            set
            {
                SetProperty(ref _isModelOpen, value);
                _ea.GetEvent<OpenCloseDialogEvent>()
                       .Publish((IsModelOpen, GetType()));
            }
        }

        DelegateCommand _CloseDialogCommand;
        public DelegateCommand CloseDialogCommand => _CloseDialogCommand ??= new(
        () =>
        {
            IsModelOpen = false;
        });
    }
}
