using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public class EmailRecipientsWixDto
    {
        [JsonProperty("contactId")]
        public string WixId { get; set; }

        public bool IsSent { get => Status == "sent"; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
