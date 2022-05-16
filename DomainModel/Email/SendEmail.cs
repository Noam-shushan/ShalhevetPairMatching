using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using PairMatching.Configuration;

namespace PairMatching.DomainModel.Email
{
    /// <summary>
    /// Send email from the system.<br/>
    /// Can send html/css template and open email with attachment.
    /// </summary>
    public class SendEmail
    {
        readonly MailSettings _mailSettings;

        const int CHUNK_SIZE = 15;

        public SendEmail(MyConfiguration configurations)
        {
            _mailSettings = configurations.MailSettings;
        }

        /// <summary>
        /// The destination address of the email
        /// </summary>
        private List<string> _to = new();

        /// <summary>
        /// The email subject
        /// </summary>
        private string _subject = "";

        /// <summary>
        /// The template of the email body
        /// </summary>
        private string _body = "";

        /// <summary>
        /// Set the destination address of the email
        /// </summary>
        /// <param name="to">email address</param>
        /// <returns>this email sender</returns>
        public SendEmail To(params string[] to)
        {
            var validAdrress = new List<string>();
            foreach (var addr in to)
            {
                if(EmailValidator.Validate(addr) == EmailAddressStatus.Valid)
                {
                    validAdrress.Add(addr.Trim());
                }
            }
            _to = validAdrress;
            return this;
        }

        /// <summary>
        /// Set the email subject
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <returns>this sender</returns>
        public SendEmail Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        /// <summary>
        /// Set the email template body
        /// </summary>
        /// <param name="template">The email template body</param>
        /// <returns>This sender</returns>
        public SendEmail Body(string body)
        {
            _body = body;
            return this;
        }

        /// <summary>
        /// Send an open email with the option to add a file attachment
        /// </summary>
        /// <param name="fileAttachment">file name to attach to the email</param>
        /// <returns></returns>
        public async Task SendOpenMailAsync(IEnumerable<string> fileAttachments = null)
        {
            if (_to?.Count == 0)
            {
                throw new Exception("Missing destination address to send email");
            }

            try
            {
                using var client = await GetSmtpClienet();

                IEnumerable<MailboxAddress> listOfAddress = GetAddresses();
                
                var message = GetMailMessage(fileAttachments);

                while (listOfAddress.Any())
                {
                    message.To.Clear();
                    var temp = listOfAddress.Take(CHUNK_SIZE);
                    message.To.AddRange(temp);
                    listOfAddress = listOfAddress.Skip(CHUNK_SIZE);
                    await client.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private MimeMessage GetMailMessage(IEnumerable<string> fileAttachments)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress(_mailSettings.UserName, _mailSettings.From);
            message.From.Add(from);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = _body,
            };
            SetAttachments(bodyBuilder, fileAttachments);

            message.Body = bodyBuilder.ToMessageBody();
            message.Subject = _subject;
            return message;
        }


        private IEnumerable<MailboxAddress> GetAddresses()
        {
            var listOfAddress = new List<MailboxAddress>();
            foreach (var addr in _to)
            {
                try
                {
                    listOfAddress.Add(new MailboxAddress("User", addr));
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return listOfAddress;
        }

        private void SetAttachments(BodyBuilder bodyBuilder, IEnumerable<string> fileAttachments)
        {
            if (fileAttachments != null)
            {
                foreach (var f in fileAttachments)
                {
                    if (!File.Exists(f))
                    {
                        throw new FileNotFoundException($"{f} File not found");
                    }
                }
                foreach (var f in fileAttachments)
                {
                    bodyBuilder.Attachments.Add(f);
                }
            }
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        private async Task SendAutoEmailAsync()
        {
            if (_to?.Count == 0)
            {
                throw new Exception("Missing destination address to send email");
            }
            try
            {
                using var client = await GetSmtpClienet();

                using var message = new MimeMessage();

                var from = new MailboxAddress(_mailSettings.UserName, _mailSettings.From);
                message.From.Add(from);

                IEnumerable<MailboxAddress> listOfAddress = GetAddresses();

                message.To.AddRange(listOfAddress);

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = _body,
                };

                message.Body = bodyBuilder.ToMessageBody();
                message.Subject = _subject;

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<SmtpClient> GetSmtpClienet()
        {
            var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.EnableSsl);

            if (!string.IsNullOrEmpty(_mailSettings.Password))
            {
                await smtpClient.AuthenticateAsync(_mailSettings.From, _mailSettings.Password);
            }

            return smtpClient;
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        public async Task SendAutoEmailAsync<T>(T model, MailTemplate template)
        {
            string result = new CreateEmailTemplate()
                .Compile(model, template.BodyTemplate);

            await Subject(template.Subject)
                .Body(result)
                .SendAutoEmailAsync();
        }
    }
}
