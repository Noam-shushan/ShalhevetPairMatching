using PairMatching.Models;
using System;
using System.Collections.Generic;

namespace PairMatching.Gui.ViewModels
{
    public class MatchingHistoriesViewModel
    {
        private List<StudentMatchingHistory> matchingHistories;

        public MatchingHistoriesViewModel(List<StudentMatchingHistory> matchingHistories)
        {
            this.matchingHistories = matchingHistories;
        }

        public DateTime DateOfRegistered { get; internal set; }
    }
}