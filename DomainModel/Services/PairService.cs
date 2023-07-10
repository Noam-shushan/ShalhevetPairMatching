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
using PairMatching.Loggin;
using ZstdSharp.Unsafe;

namespace PairMatching.DomainModel.Services
{
    public class PairService : IPairsService
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        readonly Logger _logger;

        public PairService(IDataAccessFactory dataAccessFactory, MyConfiguration config, Logger logger)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();

            _logger = logger;

            _wix = new WixDataReader(config);
        }

        public async Task VerifieyNewPairsInWix()
        {
            try
            {
                var pairs = await _unitOfWork
                        .PairsRepositry
                        .GetAllAsync(p => !p.IsDeleted 
                            && p.Status == PairStatus.Active 
                            && p.WixId != "")
                        .ConfigureAwait(false);

                var tasks = new List<Task>();
                foreach (var p in pairs)
                {
                    if (string.IsNullOrWhiteSpace(p.WixId))
                    {
                        continue;
                    }
                    var data = await _wix.VerifieyNewPair(p.WixId)
                        .ConfigureAwait(false);
                    if (data.Where(e => e.IsSent).Count() == 2)
                    {
                        p.Status = PairStatus.Learning;
                        tasks.Add(_unitOfWork.PairsRepositry.Update(p));
                    }
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
                if(tasks.Count > 0)
                    _logger.LogInformation($"Verifiey new pairs: {tasks.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Verifiey new pairs", ex);
                throw new UserException("Can not Verifiey new havrota");
            }
        }

        public async Task<IEnumerable<Pair>> GetAllPairs()
        {
            var pairs = await _unitOfWork
                .PairsRepositry
                .GetAllAsync(p => p.Status > PairStatus.Standby && !p.IsDeleted)
                .ConfigureAwait(false);

            await SetParticipaintsForEachPair(pairs)
                .ConfigureAwait(false);
            
            return pairs;
        }

        public async Task<Pair> AddNewPair(PairSuggestion pairSuggestion, PrefferdTracks track = PrefferdTracks.NoPrefrence)
        {
            try
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
                    .Update(pair.FromWorld as WorldParticipant))
                    .ConfigureAwait(false);

                var newPair = await _unitOfWork.PairsRepositry.Insert(pair)
                    .ConfigureAwait(false);

                _logger.LogInformation($"Add new pair {newPair.Id}");

                return newPair;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not add new pair {pairSuggestion.FromIsrael.Id} -> {pairSuggestion.FromWorld.Id}", ex);
                throw new UserException($"Can not add new havrota {pairSuggestion.FromIsrael.Name} -> {pairSuggestion.FromWorld.Name}");
            }
        }

        public async Task ChangeTrack(Pair pair, PrefferdTracks track)
        {
            try
            {
                pair.Track = track;
                pair.TrackHistories.Append(new TrackHistory
                {
                    Track = track,
                    DateOfUpdate = DateTime.Now
                });

                var id = await _wix.NewPair(new NewPairWixDto
                {
                    chevrutaIdFirst = pair.FromIsrael._WixId,
                    chevrutaIdSecond = pair.FromWorld._WixId,
                    date = DateTime.Now,
                    trackId = track.GetDescriptionIdFromEnum()
                }).ConfigureAwait(false);

                pair.WixId = id;

                await _unitOfWork
                        .PairsRepositry
                        .Update(pair)
                        .ConfigureAwait(false);
                
                _logger.LogInformation($"Change track to pair {pair.Id} to {track}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not change track to pair {pair.Id}, {pair.Track} -> {track}", ex);
                throw new UserException($"Can not change track to havrota {pair.FromIsrael.Name} -> {pair.FromWorld.Name}");
            }
        }

        public async Task ChangeStatus(Pair pair, PairStatus status)
        {
            try
            {
                pair.Status = status;

                await _unitOfWork
                        .PairsRepositry
                        .Update(pair)
                        .ConfigureAwait(false);

                _logger.LogInformation($"Change status to pair {pair.Id} to {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not change status to pair {pair.Id}, {pair.Status} -> {status}", ex);
                throw new UserException($"Can not change status to havrota {pair.FromIsrael.Name} -> {pair.FromWorld.Name}");
            }
        }

        public async Task ReturnToStandby(Pair pair)
        {
            pair.Status = PairStatus.Standby;

            await _unitOfWork
                     .PairsRepositry
                     .Update(pair)
                     .ConfigureAwait(false);

            _logger.LogInformation($"Return to standby for pair {pair.Id}");
        }

        public async Task DeletePair(Pair pair)
        {
            pair.IsDeleted = true;
            await UpdateParticipantsOnDelete(pair)
                .ConfigureAwait(false);
            await _unitOfWork.PairsRepositry.Update(pair)
                .ConfigureAwait(false);

            _logger.LogInformation($"Delete pair {pair.Id}");
        }

        public async Task<Pair> ActivePair(Pair pair, bool sendEmail = true)
        {
            pair.IsActive = true;
            pair.Status = PairStatus.Active;
            if (sendEmail)
            {
                var wixId = await SendNewPairToWix(pair)
                    .ConfigureAwait(false);

                pair.WixId = wixId ?? "";
            }

            await _unitOfWork.PairsRepositry.Update(pair)
                .ConfigureAwait(false);

            _logger.LogInformation($"Active pair {pair.Id}");

            return pair;
        }

        public async Task CancelPair(Pair pair)
        {
            await UpdateParticipantsOnDelete(pair)
                .ConfigureAwait(false);
            await _unitOfWork.PairsRepositry.Delete(pair.Id)
                .ConfigureAwait(false);

            _logger.LogInformation($"Censel pair {pair.Id}");
        }

        async Task UpdateParticipantsOnDelete(Pair pair)
        {
            pair.FromIsrael.MatchTo.Remove(pair.FromWorldId);
            pair.FromWorld.MatchTo.Remove(pair.FromIsraelId);
            
            await Task.WhenAll(
            _unitOfWork.IsraelParticipantsRepositry.Update(pair.FromIsrael as IsraelParticipant),
            _unitOfWork.WorldParticipantsRepositry.Update(pair.FromWorld as WorldParticipant))
                .ConfigureAwait(false);
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

        public async Task UpdatePair(Pair pair)
        {
            await _unitOfWork.PairsRepositry.Update(pair)
                .ConfigureAwait(false);
        }
        
        private async Task<string> SendNewPairToWix(Pair pair)
        {
            try
            { // 08729b80-1e6c-4598-b88d-43c12bd52409 gil and test
                var id = await _wix.NewPair(new NewPairWixDto
                {
                    chevrutaIdFirst = pair.FromIsrael._WixId,
                    chevrutaIdSecond = pair.FromWorld._WixId,
                    date = DateTime.Now,
                    trackId = pair.Track.GetDescriptionIdFromEnum()
                }).ConfigureAwait(false);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not add havrota '{pair.FromIsrael.Name} -> {pair.FromWorld.Name}' to wix", ex);
                throw new UserException($"Can not add havrota '{pair.FromIsrael.Name} -> {pair.FromWorld.Name}' to wix");
            }
        }

        public async Task<IEnumerable<StandbyPair>> GetAllStandbyPairs()
        {
            var pairs = await _unitOfWork
                .PairsRepositry
                .GetAllAsync(p => p.Status == PairStatus.Standby)
                .ConfigureAwait(false);
            
            await SetParticipaintsForEachPair(pairs)
                .ConfigureAwait(false);

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
                .GetAllAsync(p => ips.Contains(p.Id))
                .ConfigureAwait(false);

            var wps = pairs.Select(p => p.FromWorldId).Distinct();
            var worldParts = await _unitOfWork.WorldParticipantsRepositry
                .GetAllAsync(p => wps.Contains(p.Id))
                .ConfigureAwait(false);

            foreach (var p in pairs)
            {
                p.FromIsrael = israelParts.FirstOrDefault(i => i.Id == p.FromIsraelId);
                p.FromWorld = worldParts.FirstOrDefault(i => i.Id == p.FromWorldId);
            }
        }

        public async Task AddNote(Note newNote, Pair pairModel)
        {
            pairModel.Notes.Add(newNote);
            await UpdatePair(pairModel)
                .ConfigureAwait(false);
            _logger.LogInformation($"add note to pair {pairModel.Id}");
        }

        public async Task DeleteNote(Note selectedNote, Pair pair)
        {
            pair.Notes.Remove(selectedNote);
            await UpdatePair(pair)
                .ConfigureAwait(false);
            _logger.LogInformation($"delete note from pair {pair.Id}");
        }
    }

}
