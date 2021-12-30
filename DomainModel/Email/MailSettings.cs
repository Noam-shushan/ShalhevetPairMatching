using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MailKit;

namespace PairMatching.DomainModel.Email
{
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
