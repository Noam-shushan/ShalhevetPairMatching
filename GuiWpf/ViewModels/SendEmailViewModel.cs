using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.Email;
using Prism.Commands;
using PairMatching.Models;
using PairMatching.DomainModel.Services;
using GuiWpf.Events;
using Prism.Events;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace GuiWpf.ViewModels
{
    public class SendEmailViewModel : BindableBase
    {
        readonly IEventAggregator _ea;
        readonly IEmailService _emailService;

        public SendEmailViewModel(IEventAggregator ea, IEmailService emailService, SendEmail sendEmail)
        {
            _ea = ea;
            _emailService = emailService;

            _ea.GetEvent<GetEmailAddressToParticipaintsEvent>()
                .Subscribe((to) =>
                {
                    if (to == null)
                    {
                        return;
                    }
                    To.Clear();
                    To.AddRange(to);
                });
        }
        
        public ObservableCollection<EmailAddress> To { get; } = new();

        public ObservableCollection<File> Attachments { get; } = new();

        #region Properties
        private string _subject;
        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        private string _body;
        public string Body
        {
            get => _body;
            set => SetProperty(ref _body, value);
        }

        private File _selectedFile;
        public File SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private bool _isLeftToRight;
        public bool IsLeftToRight
        {
            get => _isLeftToRight;
            set => SetProperty(ref _isLeftToRight, value);
        }

        private string _link = "";
        public string Link
        {
            get => _link;
            set => SetProperty(ref _link, value);
        }
        #endregion

        #region Commands
        DelegateCommand _addAttachmentCommand;
        public DelegateCommand AddAttachmentCommand => _addAttachmentCommand ??= new(
        () =>
        {
            AddFiles();
        }, () => false);

        DelegateCommand _removeAttachmentsCommand;
        public DelegateCommand RemoveAttachmentCommand => _removeAttachmentsCommand ??= new(
        () =>
        {
            Attachments.Remove(SelectedFile);
        }, () => false);

        DelegateCommand _cloesCommand;
        public DelegateCommand CloesCommand => _cloesCommand ??= new(
        () =>
        {
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });

        private void Reset()
        {
            Subject = Body = Link = string.Empty;
        }

        DelegateCommand _sendEmailCommand;
        public DelegateCommand SendEmailCommand => _sendEmailCommand ??= new(
        async () =>
        {
            try
            {
                _ea.GetEvent<IsSendEmailEvent>().Publish(true);
                _ea.GetEvent<CloseDialogEvent>().Publish(false);
                //await _emailSender
                //   .To(To.ToArray())
                //   .Subject(Subject)
                //   .Body(Body)
                //   .Attachments(Attachments.Select(f => f.FilePath).ToArray())
                //   .SendOpenMailAsync();
                var newEmail = await _emailService.SendEmail(new EmailModel
                {
                    Body = Body,
                    Subject = Subject,
                    To = To.ToList(),
                    HtmlBody = "",
                    Link = Link,
                    Language = IsLeftToRight ? "en" : "he"
                });
                _ea.GetEvent<IsSendEmailEvent>().Publish(false);
                _ea.GetEvent<NewEmailSendEvent>().Publish(newEmail);
            }
            catch (Exception)
            {
                throw;
            }
        }); 
        #endregion


        private void AddFiles()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = true
            };

            var response = openFileDialog.ShowDialog();
            if (!openFileDialog.CheckPathExists)
            {
                //Messages.MessageBoxError("קובץ לא קיים");
            }
            if (response == true)
            {
                var temp = openFileDialog.SafeFileNames
                .Zip(openFileDialog.FileNames,
                    (fn, fp) => new File
                    {
                        FileName = fn,
                        FilePath = fp
                    });
                Attachments.AddRange(temp);
            }
        }

    }

    public record File
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}
