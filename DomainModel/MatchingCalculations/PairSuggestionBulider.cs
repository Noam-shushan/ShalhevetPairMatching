using PairMatching.Models;
using System;
using System.Linq;

namespace PairMatching.DomainModel.MatchingCalculations
{
    class PairSuggestionBulider
    {
        readonly IsraelParticipant _ip;
        readonly WorldParticipant _wp;

        PairSuggestion _result;
        
        public PairSuggestionBulider(IsraelParticipant ip, WorldParticipant wp)
        {
            _ip = ip;
            _wp = wp;

            _result = new PairSuggestion
            {
                FromIsrael = _ip,
                FromWorld = _wp
            };
        }
        
        public PairSuggestion Build()
        {
            _result.IsGenderMatch = IsGenderMatch();
            _result.IsEnglishLevelMatch = IsEnglishLevelMatch();
            _result.IsSkillLevelMatch = IsLerningSkillMatch();
            _result.IsLearningStyleMatch = IsLearningStyleMatch();
            _result.IsTrackMatch = IsTrackMatch();
            
            if (_result.IsMinmunMatch)
            {
                _result.MatchingScore = CalculateMatchingScore();
                return _result;
            }
            return null;
        }

        private int CalculateMatchingScore()
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
