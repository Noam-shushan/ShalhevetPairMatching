using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public class UpdateParticipantOnWixDto
    {
        public string _id { get; set; }

        public string fullName { get; set; }

        public string email { get; set; }

        public string tel { get; set; }

        public List<string> preferredTrack { get; set; }

        public string chevrutaId { get; set; }
    }
}
