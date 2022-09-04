using PairMatching.DataAccess.UnitOfWork;
using PairMatching.Models;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Configurations;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Tools;
using PairMatching.Models.Dtos;
using PairMatching.DomainModel.BLModels;

namespace PairMatching.DomainModel.Services
{
    public class PairService : IPairsService
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        public PairService(IDataAccessFactory dataAccessFactory, MyConfiguration config)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();

            _wix = new WixDataReader(config);
        }    
        
        // Get the most preffred track from pairs repositry 
        public async Task<PrefferdTracks> GetMostPrefferdTracks()
        {
            var pairs = await GetAllPairs();
            var mostPrefferdTracks = pairs.GroupBy(p => p.Track)
             .OrderByDescending(g => g.Count())
             .FirstOrDefault()
             .Key;
            return mostPrefferdTracks;
        }
        
        public async Task<IEnumerable<Pair>> GetAllPairs()
        {
            var pairs = await _unitOfWork
                .PairsRepositry
                .GetAllAsync();

            await SetParticipaintsToEachPair(pairs);

            return pairs;
        }

        public async Task<Pair> AddNewPair(PairSuggestion pairSuggestion)
        {
            var pair = new Pair
            {
                Track = pairSuggestion.PrefferdTrack,
                FromIsraelId = pairSuggestion.FromIsrael.Id,
                FromWorldId = pairSuggestion.FromWorld.Id,
                Status = PairStatus.Standby,
                DateOfCreate = DateTime.Now,
                IsDeleted = false,
                TrackHistories = new List<TrackHistory>()
                {
                    new TrackHistory()
                    {
                        DateOfUpdate = DateTime.Now,
                        Track = pairSuggestion.PrefferdTrack
                    }
                }
            };
            
            pairSuggestion.FromIsrael.MatchTo.Add(pair.FromWorldId);
            pairSuggestion.FromWorld.MatchTo.Add(pair.FromIsraelId);

            await Task.WhenAll(_unitOfWork
                .IsraelParticipantsRepositry
                .Update(pairSuggestion.FromIsrael as IsraelParticipant), 
                _unitOfWork
                .WorldParticipantsRepositry
                .Update(pairSuggestion.FromWorld as WorldParticipant));
            
            var newPair = await _unitOfWork.PairsRepositry.Insert(pair);

            //await SendNewPairToWix(pairSuggestion);
            // "630c879ff16774f4658c916a"
            return newPair;
        }

        private async Task SendNewPairToWix(PairSuggestion pairSuggestion)
        {
            await _wix.NewPair(new NewPairWixDto
            {
                chevrutaIdFirst = pairSuggestion.FromIsrael.WixId,
                chevrutaIdSecond = pairSuggestion.FromWorld.WixId,
                date = DateTime.Now,
                trackId = pairSuggestion.PrefferdTrack.GetDescriptionIdFromEnum()
            });
        }

        public async Task<IEnumerable<StandbyPair>> GetAllStandbyPairs()
        {
            var pairs = await _unitOfWork
                .PairsRepositry
                .GetAllAsync(p => p.Status == PairStatus.Standby);
            
            await SetParticipaintsToEachPair(pairs);

            var fly = new TimeIntervalFactory();

            return from p in pairs
                   select new StandbyPair
                   {
                       Pair = p,
                       PairSuggestion = new PairSuggestionBulider(p.FromIsrael as IsraelParticipant,
                       p.FromWorld as WorldParticipant, fly)
                       .Build()
                   };

        }

        private async Task SetParticipaintsToEachPair(IEnumerable<Pair> pairs)
        {
            if (!pairs.Any())
                return;
            
            var ips = pairs.Select(p => p.FromIsraelId).Distinct();
            var israelParts = await _unitOfWork.IsraelParticipantsRepositry
                .GetAllAsync(p => ips.Contains(p.Id));

            var wps = pairs.Select(p => p.FromWorldId).Distinct();
            var worldParts = await _unitOfWork.WorldParticipantsRepositry
                .GetAllAsync(p => wps.Contains(p.Id));

            foreach (var p in pairs)
            {
                p.FromIsrael = israelParts.FirstOrDefault(i => i.Id == p.FromIsraelId);
                p.FromWorld = worldParts.FirstOrDefault(i => i.Id == p.FromWorldId);
            }
        }
    }

}
