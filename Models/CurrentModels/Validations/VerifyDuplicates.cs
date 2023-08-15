using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.CurrentModels.Validations
{
    public static class VerifyDuplicates
    {

        public static void RemoveDuplicateTracks(this Participant participant)
        {
            if(participant.PairPreferences.Tracks.Count() != 
                participant.PairPreferences.Tracks.Distinct().Count())
            {
                participant.PairPreferences.Tracks =
                    participant.PairPreferences.Tracks.Distinct();
            }
        }

        public static void RemoveDuplicateLearningTime(this Participant participant)
        {
            if (participant.PairPreferences
                .LearningTime
                .Any(
                    lt => lt.TimeInDay.Count() != lt.TimeInDay.Distinct().Count()))
            {
                participant.PairPreferences.LearningTime = from lt in participant.PairPreferences.LearningTime
                                                           group lt by lt.Day into day
                                                           select new LearningTime
                                                           {
                                                               Day = day.Key,
                                                               TimeInDay = day.SelectMany(t => t.TimeInDay).Distinct()
                                                           };
            }
        }
    }
}
