using PairMatching.DomainModel.Properties;
using System.Text;

namespace PairMatching.DomainModel.Email
{
    /// <summary>
    /// Email template.
    /// </summary>
    public class MailTemplate
    {
        public string Subject { get; set; }

        public StringBuilder Template { get; set; }
    }

    /// <summary>
    /// Automatic email types enum
    /// </summary>
    public enum EmailTypes { SuccessfullyRegistered, YouGotPair, PairBroke, ToSecretaryNewPair, ToSecretaryPairBroke, StatusQuiz }

    /// <summary>
    /// Automatic read only mail templates
    /// </summary>
    internal static class Templates
    {
        public static MailTemplate SuccessfullyRegisteredHebrew { get; } = new MailTemplate
        {
            Subject = "נרשמת בהצלחה לתכנית שלהבת!",
            Template = new StringBuilder()
                .Append(Resources.SuccessfullyRegisteredHebrew)
        };

        public static MailTemplate SuccessfullyRegisteredEnglish { get; } = new MailTemplate
        {
            Subject = "You have successfully registered to \"Shalhevet\"!",
            Template = new StringBuilder()
                    .Append(Resources.SuccessfullyRegisteredEnglish)
        };

        public static MailTemplate YouGotPairHebrew { get; } = new MailTemplate
        {
            Subject = "יש לך חברותא בשלהבת!",
            Template = new StringBuilder()
                .Append(Resources.YouGotPairHebrew)
        };

        public static MailTemplate YouGotPairEnglish { get; } = new MailTemplate
        {
            Subject = "You\'ve got a \"Shalhevet\" learning partner!",
            Template = new StringBuilder()
                    .Append(Resources.YouGotPairEnglish)
        };

        public static MailTemplate ToSecretaryNewPair { get; } = new MailTemplate
        {
            Subject = "חברותא חדשה",
            Template = new StringBuilder()
                    .Append(Resources.ToSecretaryNewPair)
        };

        public static MailTemplate ToSecretaryPairBroke { get; } = new MailTemplate
        {
            Subject = "חברותא התפרקה",
            Template = new StringBuilder()
                    .Append(Resources.ToSecretaryPairBroke)
        };

        public static MailTemplate PairBrokeHebrew { get; } = new MailTemplate
        {
            Subject = "הצטערנו לשמוע שהחברותא הסתיימה",
            Template = new StringBuilder()
                    .Append(Resources.PairBrokeHebrew)
        };

        public static MailTemplate PairBrokeEnglish { get; } = new MailTemplate
        {
            Subject = "We are sorry to hear that your Chevruta ended",
            Template = new StringBuilder()
                .Append(Resources.PairBrokeEnglish)
        };

        public static MailTemplate StatusQuizHebrew { get; } = new MailTemplate
        {
            Subject = "שאלון תמונת מצב תקופתית",
            Template = new StringBuilder()
                .Append(Resources.StatusQuizHebrew)
        };

        public static MailTemplate StatusQuizEnglish { get; } = new MailTemplate
        {
            Subject = "Chevruta update and feedback",
            Template = new StringBuilder()
                        .Append(Resources.StatusQuizEnglish)
        };
    }
}