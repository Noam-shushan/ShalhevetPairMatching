using Prism.Mvvm;
using Prism.Events;
using Prism.Commands;
using GuiWpf.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        readonly IEventAggregator _ea;

        public MainWindowViewModel(IEventAggregator ea)
        {
            _ea = ea;
            
            _ea.GetEvent<IsSendEmailEvent>().Subscribe((val) => IsSendEmail = val);

            _ea.GetEvent<OpenCloseDialogEvent>()
                .Subscribe(e =>
                {
                    if (e.Item2 == typeof(FullParticipaintViewModel))
                    {
                        IsFullParticipaintOpen = e.Item1;
                    }
                    else if (e.Item2 == typeof(FullPairMatchingComparisonViewModel))
                    {
                        IsFullComparisonOpen = e.Item1;
                    }
                });
        }

        private bool _isSendEmail;
        public bool IsSendEmail
        {
            get => _isSendEmail;
            set => SetProperty(ref _isSendEmail, value);
        }

        private bool _isFullComparisonOpen;
        public bool IsFullComparisonOpen
        {
            get => _isFullComparisonOpen;
            set => SetProperty(ref _isFullComparisonOpen, value);
        }


        private bool _isFullParticipaintOpen;
        public bool IsFullParticipaintOpen
        {
            get => _isFullParticipaintOpen;
            set => SetProperty(ref _isFullParticipaintOpen, value);
        }
    }
}
