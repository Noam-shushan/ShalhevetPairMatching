using System.Collections.Generic;

namespace PairMatching.Configuration
{
    public sealed class MyConfiguration
    {
        public string ConnctionsStrings { get; set; }

        public bool IsTest { get; set; }

        public Dictionary<string, MailSettings> MailSettingsDict { get; set; }

        public MailSettings MailSettings { get => MailSettingsDict[IsTest ? "Test" : "Production"]; }

        public Dictionary<string, string> SpreadsheetsId { get; set; }
    }

    public class MailSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }

        public string From { get; set; }

        public string UserName { get; set; }

        public bool EnableSsl { get; set; }
    }
}
