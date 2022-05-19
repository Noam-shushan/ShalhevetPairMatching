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
                FromIsrael = new Participant
                {
                    Name = "נעם",
                    Email = "noam8shu@gmail.com",
                },
                FromWorld = new Participant
                {
                    Name = "Sara",
                    Email = "noamSararuth@gmail.com",
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
