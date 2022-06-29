using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.Email;
using Prism.Commands;
using PairMatching.Models;
using GuiWpf.Events;
using Prism.Events;

namespace GuiWpf.ViewModels
{
    public class SendEmailViewModel : BindableBase
    {
        readonly SendEmail _emailSender;
        readonly IEventAggregator _ea;

        public SendEmailViewModel(SendEmail emailSender, IEventAggregator ea)
        {
            _emailSender = emailSender;
            _ea = ea;
        }


        private List<EmailAddress> _to;
        public List<EmailAddress> To
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }

        private string _subject;
        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
        


        DelegateCommand _sendCommand;
        public DelegateCommand SendCommand => _sendCommand ??= new(
        () =>
        {
           
            
        });

        DelegateCommand _addAttachmentCommand;
        public DelegateCommand addAttachmentCommand => _addAttachmentCommand ??= new(
        () =>
        {
            
        });

        DelegateCommand _cloesCommand;
        public DelegateCommand CloesCommand => _cloesCommand ??= new(
        () =>
        {
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });


    }
}
