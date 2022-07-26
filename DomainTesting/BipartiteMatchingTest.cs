using NUnit.Framework;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using System;
using System.Collections.Generic;

namespace DomainTesting
{
    [TestFixture]
    public class BipartiteMatchingTest
    {
        [Test]
        public void MaxMatchingTestEdmoudnsKarp()
        {
            // Create test data
            List<PairSuggestion> pairSuggestions = CreatePairSuggestionData();
            
            // Create Participants data
            List<Participant> participants = CreateParticipaintData();
            
            // Create BipartiteMatching object
            var bipartiteMatching = new BipartiteMatching(pairSuggestions, participants);

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
            var bipartiteMatching = new BipartiteMatching(pairSuggestions, participants);

            var result = bipartiteMatching.MaxMatching();


            Assert.AreEqual(5, result);
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
