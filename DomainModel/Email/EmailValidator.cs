using System;
using System.Net.Mail;

namespace PairMatching.DomainModel.Email
{
    public enum EmailAddressStatus { Valid, NotValid, Empty }
    internal class EmailValidator
    {
        private const string testFromAddress = "test@gmail.com";

        public static EmailAddressStatus Validate(string address)
        {
            if (address == string.Empty)
            {
                return EmailAddressStatus.Empty;
            }
            try
            {
                new MailMessage(testFromAddress, address);
            }
            catch (FormatException)
            {
                return EmailAddressStatus.NotValid;
            }
            return EmailAddressStatus.Valid;
        }
    }
}