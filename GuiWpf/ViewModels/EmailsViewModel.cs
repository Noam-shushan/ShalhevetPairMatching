using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiWpf.Events;
using PairMatching.DomainModel.Services;
using Prism.Commands;
using Prism.Events;
using PairMatching.Models;

namespace GuiWpf.ViewModels
{
    public class EmailsViewModel : ViewModelBase
    {
        readonly IEventAggregator _ea;

        private IEmailService _emailService;

        readonly ExceptionHeandler _exceptionHeandler;

        public EmailsViewModel(IEmailService emailService, IEventAggregator ea, ExceptionHeandler exceptionHeandler)
        {
            _emailService = emailService;
            _ea = ea;
            _exceptionHeandler = exceptionHeandler;

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

        private EmailModel _selectedEmail;
        public EmailModel SelectedEmail
        {
            get => _selectedEmail;
            set => SetProperty(ref _selectedEmail, value);
        }

        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
        async () =>
        {
            await Refrash();
            IsInitialized = true;
        }, () =>!IsInitialized && !IsLoaded);


        DelegateCommand _ResendEmailCommand;
        public DelegateCommand ResendEmailCommand => _ResendEmailCommand ??= new(
        async () =>
        {
            if(Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך לשלוח את המייל שוב?"))
            {
                try
                {
                    _ea.GetEvent<IsSendEmailEvent>().Publish(true);
                    await _emailService.ResendEmail(SelectedEmail);
                    _ea.GetEvent<IsSendEmailEvent>().Publish(false);
                }
                catch (Exception ex)
                {
                    _exceptionHeandler.HeandleException(ex);
                }
            }
        });

        private async Task Refrash()
        {
            try
            {
                IsLoaded = true;
                await _emailService.VerifieyEmails();

                var emails = await _emailService.GetEmails();

                Emails.Init(emails, 10, EmailsFilter);
                IsLoaded = false;
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        }

        private bool EmailsFilter(EmailModel obj)
        {
            return true;
        }

    }
}
