namespace PairMatching.Models
{
    public class EmailAddress
    {
        public string ParticipantId { get; set; }
        
        public string ParticipantWixId { get; set; }

        public string Address { get; set; }

        public bool IsValid { get; set; }
    }
}