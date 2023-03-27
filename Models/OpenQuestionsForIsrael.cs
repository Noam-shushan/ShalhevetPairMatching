using System.Collections.Generic;

namespace PairMatching.Models
{
    public class OpenQuestionsForIsrael
    {
        public string PersonalTraits { get; set; } = "";

        public string BiographHeb { get; set; } = "";

        public string WhoIntroduced { get; set; } = "";

        public string WhyJoinShalhevet { get; set; } = "";

        public string AdditionalInfo { get; set; } = "";
    }

    public class OpenQuestionsForWorld
    {
        public string WhoIntroduced { get; set; } = "";

        public string AdditionalInfo { get; set; } = "";

        public List<string> HopesExpectations { get; set; } = new ();

        public string AnythingElse { get; set; } = "";

        public string ConversionRabi { get; set; } = "";

        public string RequestsFromPair { get; set; } = "";

        public string Experience { get; set; } = "";

        public string PersonalBackground { get; set; } = "";

        public string JewishAndComAff { get; set; } = "";

    }
}
