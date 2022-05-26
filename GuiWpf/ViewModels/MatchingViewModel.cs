using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using Prism.Mvvm;

namespace GuiWpf.ViewModels
{
    public class MatchingViewModel : BindableBase
    {

        public MatchingViewModel()
        {
            StageSuggestion = new PairSuggestion
            {
                FromIsrael = new IsraelParticipant
                {
                    Name = "נעם",
                    Email = "noam8shu@gmail.com",
                    OpenQuestions = new OpenQuestionsForIsrael
                    {
                        AdditionalInfo = "עליי פרטים נוספים",
                        BiographHeb = "פרטים ביוגרפיים",
                        PersonalTraits = "תחביבים",
                        WhoIntroduced = "מי הכיר לי את התוכנית",
                        WhyJoinShalhevet = "הצטרפתי כי בא לי"
                    }
                },
                FromWorld = new WorldParticipant
                {
                    Name = "Sara",
                    Email = "noamSararuth@gmail.com",
                    OpenQuestions = new OpenQuestionsForWorld
                    {
                    }
                },
                IsEnglishLevelMatch = true,
                IsGenderMatch = true,
                IsLearningStyleMatch = false,
                IsSkillLevelMatch = false,
                MatchingTimes = new MatchingTimes
                {
                    Sunday = new List<MatchHours>
                    {
                        new()
                        {
                            IsHoursMatch = true,
                            MatchHoursIsrael = "ערב",
                            MatchHoursWorld = "בוקר"
                        }
                    },
                    Monday = new List<MatchHours>
                    {
                        new()
                        {
                            IsHoursMatch = true,
                            MatchHoursIsrael = "צהריים",
                            MatchHoursWorld = "לילה"
                        }
                    },
                    Wednesday = new List<MatchHours>
                    {
                        new()
                        {
                            IsHoursMatch = true,
                            MatchHoursIsrael = "ערב",
                            MatchHoursWorld = "בוקר"
                        }
                    },
                }
            };
        }

        PairSuggestion _stageSuggestion;
        public PairSuggestion StageSuggestion
        {
            get => _stageSuggestion;
            set
            {
                SetProperty(ref _stageSuggestion, value);
            }
        }
    }
}
