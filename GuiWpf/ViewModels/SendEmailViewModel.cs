using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Commands;
using PairMatching.Models;
using PairMatching.DomainModel.Services;
using GuiWpf.Events;
using Prism.Events;
using Microsoft.Win32;
using System.Collections.ObjectModel;


namespace GuiWpf.ViewModels
{
    public class SendEmailViewModel : ViewModelBase
    {
        readonly IEventAggregator _ea;
        readonly IEmailService _emailService;
        readonly ExceptionHeandler _exceptionHeandler;

        public SendEmailViewModel(IEventAggregator ea, IEmailService emailService, ExceptionHeandler exceptionHeandler)
        {
            _ea = ea;
            _emailService = emailService;
            _exceptionHeandler = exceptionHeandler;
        }

        public void Init(IEnumerable<EmailAddress> emailAddresses, bool isOpen)
        {
            if (emailAddresses == null)
            {
                return;
            }
            To.Clear();
            To.AddRange(emailAddresses);
            ToView.Clear();
            ToView.AddRange(To.Take(4));
            IsOpen = isOpen;
        }

        public ObservableCollection<EmailAddress> To { get; } = new();

        public ObservableCollection<File> Attachments { get; } = new();

        public ObservableCollection<EmailAddress> ToView { get; } = new();

        #region Properties

        private bool _isMinimize = false;
        public bool IsMinimize
        {
            get => _isMinimize;
            set => SetProperty(ref _isMinimize, value);
        }


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

        DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand => _closeCommand ??= new(
        () =>
        {
            Reset();
            IsOpen = false;
        });


        DelegateCommand _MinimizeCommand;
        public DelegateCommand MinimizeCommand => _MinimizeCommand ??= new(
        () =>
        {
            IsMinimize = true;
        });


        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        private void Reset()
        {
            Subject = Body = Link = string.Empty;
            To.Clear();
        }

        DelegateCommand _sendEmailCommand;
        public DelegateCommand SendEmailCommand => _sendEmailCommand ??= new(
        async () =>
        {
            try
            {
                _ea.GetEvent<IsSendEmailEvent>().Publish(true);
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
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
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
