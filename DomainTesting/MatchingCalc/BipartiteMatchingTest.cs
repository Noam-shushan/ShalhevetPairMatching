using NUnit.Framework;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWork;
using Newtonsoft.Json;
using System.Linq;
using PairMatching.Root;

namespace DomainTesting.MatchingCalc
{
    [TestFixture]
    public class BipartiteMatchingTest
    {
        readonly IUnitOfWork _db;

        public BipartiteMatchingTest()
        {
            var conf = new Startup()
                .GetConfigurations();
            _db = new UnitOfWork(conf);

        }

        [Test]
        public void MaxMatchingTestEdmoudnsKarp()
        {
            // Create test data
            List<PairSuggestion> pairSuggestions = CreatePairSuggestionData();

            // Create Participants data
            List<Participant> participants = CreateParticipaintData();

            // Create BipartiteMatching object
            var bipartiteMatching = new BipartiteMatching(pairSuggestions);

            var result = bipartiteMatching.EdmoudnsKarp();


            Assert.AreEqual("A1->B2\nA2->B4\nA3->B1\nA4->B3\nA5->B5", string.Join("\n", result));
        }

        [Test]
        public void MaxMatchingTest()
        {
            // Create test data
            List<PairSuggestion> pairSuggestions = CreatePairSuggestionData();
            // Create Participants data
            List<Participant> participants = CreateParticipaintData();
            // Create BipartiteMatching object
            var bipartiteMatching = new BipartiteMatching(pairSuggestions);

            var result = bipartiteMatching.MaxMatchingAsNumber();


            Assert.AreEqual(5, result);
        }

        [Test]
        public async Task Hung()
        {
            var israelParticipants = await _db
                 .IsraelParticipantsRepositry
                 .GetAllAsync();
            var worldParticipants = await _db
                .WorldParticipantsRepositry
                .GetAllAsync();

            var sb = new BuildSuggestions(israelParticipants.Where(i => i.IsOpenToMatch),
                worldParticipants.Where(i => i.IsOpenToMatch));

            var res = sb.FindMaxOptPairs();
            Assert.AreEqual(12, res.Count());
        }

        private static List<Participant> CreateParticipaintData()
        {
            return new List<Participant>
            {
                new Participant
                {
                    Id = "A1",
                    Country = "Israel",
                    Name = "John",
                },
                new Participant
                {
                    Id = "A2",
                    Country = "Israel",
                    Name = "John",
                },
                new Participant
                {
                    Id = "A3",
                    Country = "Israel",
                    Name = "John",
                },
                new Participant
                {
                    Id = "A4",
                    Country = "Israel",
                    Name = "John",
                },
                new Participant
                {
                    Id = "A5",
                    Country = "Israel",
                    Name = "John",
                },
                new Participant
                {
                    Id = "B1",
                    Country = "US",
                    Name = "Mary",
                },
                new Participant
                {
                    Id = "B2",
                    Country = "US",
                    Name = "Mary",
                },
                new Participant
                {
                    Id = "B3",
                    Country = "US",
                    Name = "Mary",
                },
                new Participant
                {
                    Id = "B4",
                    Country = "US",
                    Name = "Mary",
                },
                new Participant
                {
                    Id = "B5",
                    Country = "US",
                    Name = "Mary",
                },
            };
        }

        private static List<PairSuggestion> CreatePairSuggestionData()
        {
            return new List<PairSuggestion>
            {
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A1",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B2",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A1",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B3",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A2",
                        Country = "Israel",
                        Name = "John" },
                    FromWorld = new Participant
                    {
                        Id = "B2",
                        Country = "US",
                        Name = "Mary"
                    }
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A2",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B3",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A2",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B4",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A3",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B1",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A3",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B2",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A3",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B3",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A3",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B5",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A4",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B3",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A5",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B3",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A5",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B4",
                        Country = "US",
                        Name = "Mary",
                    },
                },
                new PairSuggestion
                {
                    FromIsrael = new Participant
                    {
                        Id = "A5",
                        Country = "Israel",
                        Name = "John",
                    },
                    FromWorld = new Participant
                    {
                        Id = "B5",
                        Country = "US",
                        Name = "Mary",
                    },
                },
            };
        }
    }
}
