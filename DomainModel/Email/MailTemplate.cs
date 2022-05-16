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

        public string BodyTemplate { get; set; }
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
            BodyTemplate = Resources.SuccessfullyRegisteredHebrew
        };

        public static MailTemplate SuccessfullyRegisteredEnglish { get; } = new MailTemplate
        {
            Subject = "You have successfully registered to \"Shalhevet\"!",
            BodyTemplate = Resources.SuccessfullyRegisteredEnglish
        };

        public static MailTemplate YouGotPairHebrew { get; } = new MailTemplate
        {
            Subject = "יש לך חברותא בשלהבת!",
            BodyTemplate = Resources.YouGotPairHebrew
        };

        public static MailTemplate YouGotPairEnglish { get; } = new MailTemplate
        {
            Subject = "You\'ve got a \"Shalhevet\" learning partner!",
            BodyTemplate = Resources.YouGotPairEnglish
        };

        public static MailTemplate ToSecretaryNewPair { get; } = new MailTemplate
        {
            Subject = "חברותא חדשה",
            BodyTemplate = Resources.ToSecretaryNewPair
        };

        public static MailTemplate ToSecretaryPairBroke { get; } = new MailTemplate
        {
            Subject = "חברותא התפרקה",
            BodyTemplate = Resources.ToSecretaryPairBroke
        };

        public static MailTemplate PairBrokeHebrew { get; } = new MailTemplate
        {
            Subject = "הצטערנו לשמוע שהחברותא הסתיימה",
            BodyTemplate = Resources.PairBrokeHebrew
        };

        public static MailTemplate PairBrokeEnglish { get; } = new MailTemplate
        {
            Subject = "We are sorry to hear that your Chevruta ended",
            BodyTemplate = Resources.PairBrokeEnglish
        };

        public static MailTemplate StatusQuizHebrew { get; } = new MailTemplate
        {
            Subject = "שאלון תמונת מצב תקופתית",
            BodyTemplate = Resources.StatusQuizHebrew
        };

        public static MailTemplate StatusQuizEnglish { get; } = new MailTemplate
        {
            Subject = "Chevruta update and feedback",
            BodyTemplate = Resources.StatusQuizEnglish
        };
    }
}