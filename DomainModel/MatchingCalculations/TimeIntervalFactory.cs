using PairMatching.Models;
using System;
using System.Collections.Generic;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class TimeIntervalFactory
    {
        readonly Dictionary<(TimesInDay, Days), TimeInterval> _timeToInterval = new();
        
        public TimeInterval FromTimeInDay(TimesInDay timeInDay, Days eDay)
        {
            if (!_timeToInterval.ContainsKey((timeInDay, eDay)))
            {
                if (eDay == Days.Defulte || timeInDay == TimesInDay.Defulte || timeInDay == TimesInDay.Incapable)
                {
                    return null;
                }
                int day = (int)eDay;
                switch (timeInDay)
                {
                    case TimesInDay.Morning:
                        _timeToInterval.Add((timeInDay, eDay), 
                            new TimeInterval(TimeSpan.Parse("05:00"), TimeSpan.Parse("12:00"), day));
                        break;
                    case TimesInDay.Noon:
                        _timeToInterval.Add((timeInDay, eDay), 
                            new TimeInterval(TimeSpan.Parse("12:00"), TimeSpan.Parse("18:00"), day));
                        break;
                    case TimesInDay.Evening:
                        _timeToInterval.Add((timeInDay, eDay), 
                            new TimeInterval(TimeSpan.Parse("18:00"), TimeSpan.Parse("21:00"), day));
                        break;
                    case TimesInDay.Night:
                        _timeToInterval.Add((timeInDay, eDay), 
                            new TimeInterval(TimeSpan.Parse("21:00"), TimeSpan.Parse("1.02:00"), day));
                        break;
                    default:
                        throw new ArgumentException("Invalid time in day");
                }
            }
            return _timeToInterval[(timeInDay, eDay)];
        }
    }
}