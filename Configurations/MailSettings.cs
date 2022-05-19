namespace PairMatching.Configurations
{
    public class MailSettings
    {
        public string Host { get; init; }

        public int Port { get; init; }

        public string Password { get; init; }

        public string From { get; init; }

        public string UserName { get; init; }

        public bool EnableSsl { get; init; }
    }
}
