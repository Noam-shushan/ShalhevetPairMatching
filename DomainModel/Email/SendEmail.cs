using MailKit.Net.Smtp;
using MimeKit;
using PairMatching.Configurations;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Email
{
    /// <summary>
    /// Send email from the system.<br/>
    /// Can send html/css template and open email with attachment.
    /// </summary>
    public class SendEmail
    {
        readonly MailSettings _mailSettings;

        const int CHUNK_SIZE = 1;

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

        List<string> _attachments = new();

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
                if (EmailValidator.Validate(addr) == EmailAddressStatus.Valid)
                {
                    validAdrress.Add(addr.Trim());
                }
            }
            _to = validAdrress;
            return this;
        }

        public SendEmail To(params EmailAddress[] to)
        {
            var validAdrress = new List<string>();
            foreach (var addr in to)
            {
                if (EmailValidator.Validate(addr.Address) == EmailAddressStatus.Valid)
                {
                    validAdrress.Add(addr.Address.Trim());
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

        public SendEmail Attachments(params string[] attachments)
        {
            _attachments = attachments.ToList();
            return this;
        }

        /// <summary>
        /// Send an open email with the option to add a file attachment
        /// </summary>
        /// <returns></returns>
        public async Task SendOpenMailAsync()
        {
            if (_to?.Count == 0)
            {
                throw new Exception("Missing destination address to send email");
            }

            try
            {
                using var client = await GetSmtpClienet();

                IEnumerable<MailboxAddress> listOfAddress = GetAddresses();

                var message = GetMailMessage();

                while (listOfAddress.Any())
                {
                    message.To.Clear();
                    var temp = listOfAddress.Take(CHUNK_SIZE);
                    message.To.AddRange(temp);
                    listOfAddress = listOfAddress.Skip(CHUNK_SIZE);
                    try
                    {
                        await client.SendAsync(message);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private MimeMessage GetMailMessage()
        {
            var message = new MimeMessage();

            var from = new MailboxAddress(_mailSettings.UserName, _mailSettings.From);
            message.From.Add(from);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = _body,
            };
            if (_attachments.Any())
            {
                SetAttachments(bodyBuilder);
            }


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

        private void SetAttachments(BodyBuilder bodyBuilder)
        {
            foreach (var f in _attachments)
            {
                if (!File.Exists(f))
                {
                    throw new FileNotFoundException($"{f} File not found");
                }
            }
            foreach (var f in _attachments)
            {
                bodyBuilder.Attachments.Add(f);
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
