using System.Collections.Generic;

namespace PairMatching.Models.Dtos
{
    public class UpsertParticipantOnWixDtoNoId
    {

        public string fullName { get; set; } = "";

        public string email { get; set; } = "";

        public string tel { get; set; } = "";

        public List<string> preferredTrack { get; set; }

        public string country { get; set; } = "";

        public string learningSkill { get; set; } = "";

        public string gender { get; set; } = "";

        public string menOrWomen { get; set; } = "";

        public string levOfEn { get; set; } = "";

        public string chevrotalevOfEn { get; set; } = "";

        public string moreThanOneChevruta { get; set; } = "";

        public string chevrotaSkills { get; set; } = "";

        public int timeOffset { get; set; } = 0;
        
        public string learningStyle { get; set; } = "";

        public bool isNew { get; set; }
        
        public List<string> sunday { get; set; } = new();
        public List<string> thurseday { get; set; } = new();
        public List<string> tuseday { get; set; } = new();
        public List<string> wednesday { get; set; } = new();
        public List<string> monday { get; set; } = new();
        public List<string> otherLan { get; set; } = new();
    }
}