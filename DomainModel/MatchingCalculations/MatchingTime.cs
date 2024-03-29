﻿using PairMatching.Models;
using System;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class MatchingTime
    {
        public Days IsraelDay { get; set; }
        
        public Days WorldDay { get; set; }

        public TimesInDay HoursIsrael { get; set; }
        
        public TimesInDay HoursWorld { get; set; }

        public TimeSpan TotalMatchTime { get; set; }
    }
}
