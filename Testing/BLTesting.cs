using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    internal class BipartiteMatchingTesting
    {
        // Create test for BipartiteMatching class
        public void BipartiteMatchingTest()
        {
            // Create test data
            List<PairSuggestion> pairSuggestions = CreatePairSuggestionData();
            // Create Participants data
            List<Participant> participants = CreateParticipaintData();
            // Create BipartiteMatching object
            var bipartiteMatching = new BipartiteMatching(pairSuggestions, participants);
            
            var result = bipartiteMatching.EdmoudnsKarp();
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
                        Id = "B1",
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
                        Name = "John" },
                    FromWorld = new Participant
                    {
                        Id = "B3",
                        Country = "US",
                        Name = "Mary"
                    }
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
                        Id = "B5",
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
                        Id = "B2",
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
                        Id = "B1",
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
