using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PairMatching.DomainModel.MatchingCalculations
{
    class PairSuggestionBulider
    {
        readonly IsraelParticipant _ip;
        readonly WorldParticipant _wp;
        readonly PairSuggestion _result;

        const int minHoursToLearn = 1;

        readonly TimeIntervalFactory _intervalFactory;

        public PairSuggestionBulider(IsraelParticipant ip, WorldParticipant wp, TimeIntervalFactory timeIntervalFactory)
        {
            _ip = ip;
            _wp = wp;

            _result = new PairSuggestion
            {
                FromIsrael = _ip,
                FromWorld = _wp
            };

            _intervalFactory = timeIntervalFactory;
        }
        
        public PairSuggestion Build()
        {
            _result.IsGenderMatch = IsGenderMatch();
            _result.IsEnglishLevelMatch = IsEnglishLevelMatch();
            _result.IsSkillLevelMatch = IsLerningSkillMatch();
            _result.IsLearningStyleMatch = IsLearningStyleMatch();
            _result.IsTrackMatch = IsTrackMatch();
            _result.MatchingTimes = GetMatchingTimes().ToList();

            if (_result.IsMinmunMatch)
            {
                _result.MatchingScore = CalculateMatchingScore();
                return _result;
            }
            return null;
        }

        int CalculateMatchingScore()
        {
            int score = 0;
            
            if (_result.IsEnglishLevelMatch)
                score++;
            
            if (_result.IsSkillLevelMatch)
                score++;
            
            if (_wp.PairPreferences.LearningStyle == _ip.PairPreferences.LearningStyle)
                score++;
            
            if (_ip.PairPreferences.Gender == _wp.Gender
                && _wp.PairPreferences.Gender == _ip.Gender)
                score++;
            
            if(_ip.PairPreferences.Tracks
                .Where(t => t != PrefferdTracks.NoPrefrence)
                .Intersect(_wp.PairPreferences.Tracks)
                .Any())
                score++;

            score += _result.MatchingTimes.Count;

            return score;
        }

        IEnumerable<MatchingTime> GetMatchingTimes()
        {
            var ipTimes = GetTimeIntervals(_ip.PairPreferences.LearningTime);
            var wpTimes = GetTimeIntervals(_ip.PairPreferences.LearningTime);

            return from it in ipTimes
                   from wt in wpTimes
                   let interI = it.Item3 // the israeli time interval
                   let interW = wt.Item3 // the world time interval
                   let total = interI.FitWith(interW, _wp.DiffFromIsrael)
                   where total.Hours >= minHoursToLearn  // at least 1 hour to learn
                   select new MatchingTime
                   {
                       HoursIsrael = it.Item1,
                       HoursWorld = wt.Item1,
                       IsraelDay = it.Item2,
                       WorldDay = wt.Item2,
                       TotalMatchTime = total
                   };
        }

        IEnumerable<(TimesInDay, Days, TimeInterval)> GetTimeIntervals(IEnumerable<LearningTime> learningTimes)
        {
            return from lt in learningTimes
                   from timeInDay in lt.TimeInDay
                   select (timeInDay, lt.Day, _intervalFactory.FromTimeInDay(timeInDay, lt.Day));
        }

        bool IsGenderMatch()
        {
            return (_ip.PairPreferences.Gender == _wp.Gender
                && _wp.PairPreferences.Gender == _ip.Gender)
                || (_ip.PairPreferences.Gender == Genders.NoPrefrence
                    && _wp.PairPreferences.Gender == _ip.Gender)
                || (_wp.PairPreferences.Gender == Genders.NoPrefrence
                    && _ip.PairPreferences.Gender == _wp.Gender);
        }

        private bool IsTrackMatch()
        {
            return _ip.PairPreferences.Tracks
                .Intersect(_wp.PairPreferences.Tracks)
                .Any();
        }

        private bool IsEnglishLevelMatch()
        {
            return _ip.EnglishLevel >= _wp.DesiredEnglishLevel;
        }

        private bool IsLerningSkillMatch()
        {
            return _wp.SkillLevel >= _ip.DesiredSkillLevel;
        }

        private bool IsLearningStyleMatch()
        {
            return _wp.PairPreferences.LearningStyle == _ip.PairPreferences.LearningStyle
                || _wp.PairPreferences.LearningStyle == LearningStyles.NoPrefrence
                || _ip.PairPreferences.LearningStyle == LearningStyles.NoPrefrence;
        }
    }
}
