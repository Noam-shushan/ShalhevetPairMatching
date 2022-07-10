using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public class UpsertParticipantOnWixDto : UpsertParticipantOnWixDtoNoId
    {
        public string _id { get; set; } = "";
    }
}
