using PairMatching.DataAccess.UnitOfWorks;
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
using PairMatching.DataAccess.Repositories;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Crypto;

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
                .GetAllAsync(p => p.Status > PairStatus.Standby);

            await SetParticipaintsForEachPair(pairs);

            //await _unitOfWork.PairsRepositry.SaveToDrive();
            
            return pairs;
        }

        public async Task<Pair> AddNewPair(PairSuggestion pairSuggestion, PrefferdTracks track = PrefferdTracks.NoPrefrence)
        {
            var pair = CreateNewPair(pairSuggestion, track);

            pair.FromIsrael.MatchTo.Add(pair.FromWorldId);
            pair.FromWorld.MatchTo.Add(pair.FromIsraelId);

            await Task.WhenAll(
                _unitOfWork
                .IsraelParticipantsRepositry
                // Update the israeli participaint
                .Update(pair.FromIsrael as IsraelParticipant),
                
                _unitOfWork
                .WorldParticipantsRepositry
                // Update the world participaint
                .Update(pair.FromWorld as WorldParticipant));

            var newPair = await _unitOfWork.PairsRepositry.Insert(pair);
            
            return newPair;
        }

        public async Task DeletePair(Pair pair)
        {
            pair.IsDeleted = true;
            await UpdateParticipantsOnDelete(pair);
            await _unitOfWork.PairsRepositry.Update(pair);
        }

        public async Task<Pair> ActivePair(Pair pair)
        {
            pair.IsActive = true;
            pair.Status = PairStatus.Active;

            await SendNewPairToWix(pair);

            await _unitOfWork.PairsRepositry.Update(pair);

            return pair;
        }

        public async Task CancelPair(Pair pair)
        {
            await UpdateParticipantsOnDelete(pair);
            await _unitOfWork.PairsRepositry.Delete(pair.Id);
        }

        async Task UpdateParticipantsOnDelete(Pair pair)
        {
            pair.FromIsrael.MatchTo.Remove(pair.FromWorldId);
            pair.FromWorld.MatchTo.Remove(pair.FromIsraelId);
            
            await Task.WhenAll(
            _unitOfWork.IsraelParticipantsRepositry.Update(pair.FromIsrael as IsraelParticipant),
            _unitOfWork.WorldParticipantsRepositry.Update(pair.FromWorld as WorldParticipant));
        }

        public Pair CreateNewPair(PairSuggestion pairSuggestion, PrefferdTracks track = PrefferdTracks.NoPrefrence)
        {
            var finalTrack = track != PrefferdTracks.NoPrefrence ? track : pairSuggestion.ChosenTrack;
            return new Pair
            {
                Track = finalTrack,
                FromIsraelId = pairSuggestion.FromIsrael.Id,
                FromWorldId = pairSuggestion.FromWorld.Id,
                FromIsrael = pairSuggestion.FromIsrael,
                FromWorld = pairSuggestion.FromWorld,
                Status = PairStatus.Standby,
                DateOfCreate = DateTime.Now,
                IsDeleted = false,
                TrackHistories = new List<TrackHistory>()
                {
                    new TrackHistory()
                    {
                        DateOfUpdate = DateTime.Now,
                        Track = finalTrack
                    }
                }
            };
        }
        
        private async Task SendNewPairToWix(Pair pair)
        {
            await _wix.NewPair(new NewPairWixDto
            {
                chevrutaIdFirst = pair.FromIsrael.WixId,
                chevrutaIdSecond = pair.FromWorld.WixId,
                date = DateTime.Now,
                trackId = pair.Track.GetDescriptionIdFromEnum()
            });
        }

        public async Task<IEnumerable<StandbyPair>> GetAllStandbyPairs()
        {
            var pairs = await _unitOfWork
                .PairsRepositry
                .GetAllAsync(p => p.Status == PairStatus.Standby);
            
            await SetParticipaintsForEachPair(pairs);

            var fly = new TimeIntervalFactory();

            var result = from p in pairs
                   select new StandbyPair
                   {
                       Pair = p,
                       PairSuggestion = new PairSuggestionBulider(p.FromIsrael as IsraelParticipant,
                       p.FromWorld as WorldParticipant, fly)
                       .Build()
                   };

            return result;
        }

        private async Task SetParticipaintsForEachPair(IEnumerable<Pair> pairs)
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
