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
using PairMatching.Models;

namespace GuiWpf.ViewModels
{
    public class EmailsViewModel : ViewModelBase
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
            _ea.GetEvent<NewEmailSendEvent>()
                .Subscribe(em =>
                {
                    if (em == null)
                    {
                        return;
                    }
                    Emails.Add(em);
                });
        }

        public PaginCollectionViewModel<EmailModel> Emails { get; set; } = new();

        private bool _isSendEmailOpen = false;
        public bool IsSendEmailOpen
        {
            get => _isSendEmailOpen;
            set => SetProperty(ref _isSendEmailOpen, value);
        }


        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
        async () =>
        {
            await Refrash();
            IsInitialized = true;
        }, () =>!IsInitialized && !IsLoaded);

        private async Task Refrash()
        {
            IsLoaded = true;
            await _emailService.VerifieyEmails();

            var emails = await _emailService.GetEmails();

            Emails.Init(emails, 10, EmailsFilter);
            IsLoaded = false;
        }

        DelegateCommand _openSendCommand;
        public DelegateCommand OpenSendView => _openSendCommand ??= new(
        () =>
        {
            IsSendEmailOpen = !IsSendEmailOpen;
        });

        private bool EmailsFilter(EmailModel obj)
        {
            return true;
        }

    }
}
