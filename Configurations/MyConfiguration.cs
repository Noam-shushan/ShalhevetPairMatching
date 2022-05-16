﻿using System.Collections.Generic;

namespace PairMatching.Configuration
{
    public sealed class MyConfiguration
    {
        public string ConnctionsStrings { get; init; }

        public bool IsTest { get; init; }

        public Dictionary<string, MailSettings> MailSettingsDict { private get; init; }

        public MailSettings MailSettings { get => MailSettingsDict[IsTest ? "Test" : "Production"]; }

        public Dictionary<string, string> SpreadsheetsId { get; init; }
    }
}
