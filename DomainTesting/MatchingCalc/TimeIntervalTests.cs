using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using System.Threading.Tasks;
using static PairMatching.Tools.HelperFunction;

namespace DomainTesting.MatchingCalc
{
    [TestFixture]
    public class TimeIntervalTests
    {
        TimeIntervalFactory _intervalFactory = new();

        [Test]
        public void NegativeDiffrence_SameDay()
        {
            var i = _intervalFactory.FromTimeInDay(TimesInDay.Evening, Days.Sunday);

            var w = _intervalFactory.FromTimeInDay(TimesInDay.Morning, Days.Sunday);

            Assert.AreEqual(TimeSpan.FromHours(1), i.FitWith(w, TimeSpan.FromHours(-7)));
        }

        [Test]
        public void NegativeDiffrence_DayBefore()
        {
            var i = _intervalFactory.FromTimeInDay(TimesInDay.Morning, Days.Monday);

            var w = _intervalFactory.FromTimeInDay(TimesInDay.Night, Days.Sunday);

            Assert.AreEqual(TimeSpan.FromHours(4), i.FitWith(w, TimeSpan.FromHours(-7)));
        }

        [Test]
        public void PositiveDiffrence_SameDay()
        {
            var i = _intervalFactory.FromTimeInDay(TimesInDay.Noon, Days.Sunday);

            var w = _intervalFactory.FromTimeInDay(TimesInDay.Evening, Days.Sunday);

            Assert.AreEqual(TimeSpan.FromHours(3), i.FitWith(w, TimeSpan.FromHours(4)));
        }

        [Test]
        public void PositiveDiffrence_DayAfter()
        {
            var i = _intervalFactory.FromTimeInDay(TimesInDay.Night, Days.Sunday);

            var w = _intervalFactory.FromTimeInDay(TimesInDay.Morning, Days.Monday);

            Assert.AreEqual(TimeSpan.FromHours(4), i.FitWith(w, TimeSpan.FromHours(7)));
        }
    }
}
