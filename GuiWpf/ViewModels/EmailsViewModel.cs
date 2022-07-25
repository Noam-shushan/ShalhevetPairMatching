using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiWpf.Events;
using PairMatching.DomainModel.Services;
using PairMatching.DomainModel.Email;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace GuiWpf.ViewModels
{
    public class EmailsViewModel : BindableBase
    {
        readonly IEventAggregator _ea;

        private IEmailService _emailService;
        
        private SendEmail _emailSender;

        public EmailsViewModel(IEmailService emailService, IEventAggregator ea, SendEmail sendEmail)
        {
            _emailService = emailService;
            _ea = ea;
            _emailSender = sendEmail;

            _ea.GetEvent<CloseDialogEvent>().Subscribe((isClose) =>
            {
                IsSendEmailOpen = isClose;
            });
        }

        private bool _isSendEmailOpen = false;
        public bool IsSendEmailOpen
        {
            get => _isSendEmailOpen;
            set => SetProperty(ref _isSendEmailOpen, value);
        }

        DelegateCommand _openSendCommand;
        public DelegateCommand OpenSendView => _openSendCommand ??= new(
        () =>
        {
            IsSendEmailOpen = !IsSendEmailOpen;
        });



    }
}
