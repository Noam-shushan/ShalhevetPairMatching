using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;
using System.IO;

namespace PairMatching.DomainModel.Email
{
    /// <summary>
    /// Send email from the system.<br/>
    /// Can send html/css template and open email with attachment.
    /// </summary>
    public class SendEmail
    {
        readonly MailSettings _mailSettings;

        public SendEmail(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        /// <summary>
        /// The destination address of the email
        /// </summary>
        private string _to = "";

        /// <summary>
        /// The email subject
        /// </summary>
        private string _subject = "";

        /// <summary>
        /// The template of the email body
        /// </summary>
        private StringBuilder _template = new StringBuilder();

        /// <summary>
        /// Set the destination address of the email
        /// </summary>
        /// <param name="to">email address</param>
        /// <returns>this email sender</returns>
        public SendEmail To(string to)
        {
            _to = to;
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
        public SendEmail Template(StringBuilder template)
        {
            _template = template;
            return this;
        }

        /// <summary>
        /// Send an open email with the option to add a file attachment
        /// </summary>
        /// <param name="fileAttachment">file name to attach to the email</param>
        /// <returns></returns>
        public async Task SendOpenMailAsync(IEnumerable<string> fileAttachments = null)
        {
            if (_to == string.Empty)
            {
                //throw new Exception("Missing destination address to send email");
                return;
            }

            await SendOpenEmailToOne(fileAttachments);
        }

        private async Task SendOpenEmailToOne(IEnumerable<string> fileAttachments)
        {
            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.EnableSsl);

                if (!string.IsNullOrEmpty(_mailSettings.Password))
                {
                    await client.AuthenticateAsync(_mailSettings.From, _mailSettings.Password);
                }

                using var message = new MimeMessage();

                var from = new MailboxAddress(_mailSettings.UserName, _mailSettings.From);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("User", _to);
                message.To.Add(to);

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = _template.ToString(),
                };
                SetAttachments(bodyBuilder, fileAttachments);

                message.Body = bodyBuilder.ToMessageBody();
                message.Subject = _subject;

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
    }
}
