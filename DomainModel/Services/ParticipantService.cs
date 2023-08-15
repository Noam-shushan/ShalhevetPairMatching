using Newtonsoft.Json;
using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWorks;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Models;
using PairMatching.Models.Dtos;
using PairMatching.Tools;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PairMatching.Loggin;
using static PairMatching.Tools.HelperFunction;
using static PairMatching.Models.CurrentModels.Validations.VerifyDuplicates;


namespace PairMatching.DomainModel.Services
{
    public class ParticipantService : IParticipantService
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        readonly Logger _logger;

        readonly string _configId;

        public ParticipantService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration, Logger logger)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();

            _wix = new WixDataReader(configuration);

            _logger = logger;

            _configId = configuration.ConfigIdInMongo;
        }

        public async Task<IEnumerable<Participant>> GetAll()
        {
            var result = new List<Participant>();
            var tasks = new List<Task>();

            var ips = GetAllFromIsrael();
            var wps = GetAllFromWorld();


            tasks.Add(ips);
            tasks.Add(wps);

            await Task.WhenAll(tasks)
                .ConfigureAwait(false);

            result.AddRange(ips.Result);
            result.AddRange(wps.Result);

            return result;
        }

        public async Task<IEnumerable<IsraelParticipant>> GetAllFromIsrael()
        {
            return await _unitOfWork
                .IsraelParticipantsRepositry
                .GetAllAsync(ip => !ip.IsDeleted && !ip.IsInArchive)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorldParticipant>> GetAllFromWorld()
        {
            return await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync(wp => !wp.IsDeleted && !wp.IsInArchive)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Participant>> GetArchive()
        {
            var result = new List<Participant>();
            
            var ips = _unitOfWork
                .IsraelParticipantsRepositry
                .GetAllAsync(ip => ip.IsInArchive);
            
            var wps = _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync(wp => wp.IsInArchive);
             
            await Task.WhenAll(ips, wps)
                .ConfigureAwait(false);

            result.AddRange(ips.Result);
            result.AddRange(wps.Result);
            return result;
        }

        public async Task SetNewParticipintsFromWix()
        {
            var config = await _unitOfWork.ConfigRepositry
            .GetDbConfig(_configId).ConfigureAwait(false);

            var max = config?.WixIndex;
            int temp = max ?? WixDataReader.MinIndex;
            int index = temp < WixDataReader.MinIndex ? WixDataReader.MinIndex : temp;

            IEnumerable<ParticipantWixDto> partsDtos;
            try
            {
                partsDtos = await _wix.GetNewParticipants(index)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while trying to get new members from wix", ex);
                throw new UserException($"Sory... there is some error\nDo not worry the develper is informed");
            }

            if (partsDtos.Any())
            {

                var list = partsDtos.ToLookup(p => p is IsraelParticipantWixDto);
                
                var ips = list[true]
                        .Select(p => (p as IsraelParticipantWixDto).ToIsraelParticipant());

                var wps = list[false]
                        .Select(p => (p as WorldParticipantWixDto).ToWorldParticipant());

                var countryUts = GetCountryUtcs();
                foreach (var wp in wps)
                {
                    var utc = countryUts.FirstOrDefault(uc => CompereOnlyLetters(uc.Country, wp.Country));
                    if (utc != null && utc.UtcOffset != wp.UtcOffset)
                    {
                        wp.UtcOffset = utc.UtcOffset;
                    }
                }
                
                if (ips.Any())
                {
                    await _unitOfWork.IsraelParticipantsRepositry
                            .InsertMany(ips).ConfigureAwait(false);
                }

                if (wps.Any())
                {
                    await _unitOfWork.WorldParticipantsRepositry
                            .InsertMany(wps).ConfigureAwait(false);
                }
                
                var names = partsDtos.Select(p => p.fullName);
                _logger.LogInformation($"New members added: {string.Join(", ", names)}, count: {partsDtos.Count()}");
                
                int newMaxIndex = partsDtos.Max(p => p.index);
                if(newMaxIndex != config.WixIndex)
                {
                    config.WixIndex = newMaxIndex;
                    await _unitOfWork.ConfigRepositry
                        .UpdateDbConfig(config)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task AddNote(Note note, Participant participant)
        {
            participant.Notes.Add(note);
            await UpdateParticipaint(participant).ConfigureAwait(false);
            _logger.LogInformation($"Note added to participant: {participant.Id} by {note.Author}");
        }

        
        public async Task UpdateParticipaint(Participant participant, bool isChaengCountery = false)
        {
            try
            {
                if (isChaengCountery)
                {
                    try
                    {
                        await _unitOfWork
                        .IsraelParticipantsRepositry
                        .Delete(participant.Id);
                    }
                    catch (KeyNotFoundException)
                    {
                        await _unitOfWork
                            .WorldParticipantsRepositry
                            .Delete(participant.Id);
                    }
                    _logger.LogInformation($"participant {participant.Name}, {participant.Email} is cahnge country");
                }
                participant.RemoveDuplicateLearningTime();
                participant.RemoveDuplicateTracks();
                if (participant is IsraelParticipant ip)
                {
                    await _unitOfWork
                        .IsraelParticipantsRepositry
                        .Update(ip)
                        .ConfigureAwait(false);
                }
                else if (participant is WorldParticipant wp)
                {
                    await _unitOfWork
                        .WorldParticipantsRepositry
                        .Update(wp)
                        .ConfigureAwait(false);
                }
                _logger.LogInformation($"participaint {participant.Id} get update");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not update member '{participant.Id}'", ex);
                throw new UserException($"Sory... there is some error\nDo not worry the develper is informed");
            }
        }
       
        
        public async Task<Participant> InsertParticipant(Participant part)
        {
            WixIdDto wixId = null; 
            try
            {
                wixId = await GetWixId(part)
                    .ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while trying to add '{part.Name}' to wix", ex);
                throw new UserException($"Error while trying to add '{part.Name}' to wix");
            }

            part.WixId = wixId.Id;
            part._WixId = wixId._Id;

            Participant result = new();

            try
            {
                if (part is IsraelParticipant ip)
                {
                    result = await _unitOfWork
                    .IsraelParticipantsRepositry
                    .Insert(ip)
                    .ConfigureAwait(false);
                }
                else if (part is WorldParticipant wp)
                {
                    result = await _unitOfWork
                    .WorldParticipantsRepositry
                    .Insert(wp)
                    .ConfigureAwait(false);
                }
                _logger.LogInformation($"new participaint added {result.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not insert member '{part.Name}'", ex);
                throw new UserException($"Sory... there is some error\nDo not worry the develper is informed");
            }

            return result;   
        }
        
        public async Task DeleteParticipaint(Participant participant)
        {
            await RemoveMatchParticipaints(participant)
                .ConfigureAwait(false);

            participant.IsDeleted = true;

            await UpdateParticipaint(participant)
                .ConfigureAwait(false);

            _logger.LogInformation($"Delete participaint {participant.Id}");
        }

        public async Task SendToArcive(Participant participant)
        {
            await RemoveMatchParticipaints(participant)
                .ConfigureAwait(false);
            
            participant.IsInArchive = true;

            await UpdateParticipaint(participant)
                .ConfigureAwait(false);

            _logger.LogInformation($"Send participaint {participant.Id} to archiv");
        }

        private async Task RemoveMatchParticipaints(Participant participant)
        {
            try
            {
                var matchParticipants = new List<Participant>();
                foreach (var matchParticipant in participant.MatchTo)
                {
                    Participant p;
                    if (participant.IsFromIsrael)
                    {
                        p = await _unitOfWork
                            .WorldParticipantsRepositry
                            .GetByIdAsync(matchParticipant)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        p = await _unitOfWork
                            .IsraelParticipantsRepositry
                         .GetByIdAsync(matchParticipant)
                         .ConfigureAwait(false);
                    }
                    matchParticipants.Add(p);
                }
                var tasks = new List<Task>();
                foreach (var p in matchParticipants)
                {
                    p.MatchTo.Remove(participant.Id);
                    if (participant.IsFromIsrael)
                    {
                        tasks.Add(_unitOfWork.WorldParticipantsRepositry
                            .Update(p as WorldParticipant));
                    }
                    else
                    {
                        tasks.Add(_unitOfWork.IsraelParticipantsRepositry
                            .Update(p as IsraelParticipant));
                    }
                }
                await Task.WhenAll(tasks)
                    .ConfigureAwait(false);
                
                _logger.LogInformation($"Member '{participant.Name}' is unmatch");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not unmatch member '{participant.Name}'", ex);
                throw new UserException($"Sory... there is some error\nDo not worry the develper is informed");
            }
        }

        private async Task<dynamic> GetWixId(Participant part)
        {
            dynamic partDto = new { };
            if (part is IsraelParticipant ip)
            {
                partDto = ip.ToIsraelParticipantWixDto();
            }
            else if (part is WorldParticipant wp)
            {
                partDto = wp.ToWorldParticipantWixDto();
            }

            var wixId = await _wix.NewParticipant(partDto);
            return wixId;
        }


        IEnumerable<CountryUtc> _countryUtcs;
        public IEnumerable<CountryUtc> GetCountryUtcs()
        {
            _countryUtcs ??= Init();
            return _countryUtcs;

            static IEnumerable<CountryUtc> Init()
            {
                var jsonString = ReadJson(@"Resources\Countries.json");
                var result = JsonConvert.DeserializeObject<IEnumerable<CountryUtc>>(jsonString);
                foreach (var country in result)
                {
                    country.UtcOffset = country.UtcTimeOffset.ToTimeSpan();
                }
                return result;
            }
        }

        public async Task DeleteNote(Note selectedNote, Participant participant)
        {
            participant.Notes.Remove(selectedNote);
            await UpdateParticipaint(participant)
                .ConfigureAwait(false);
            _logger.LogInformation($"Delete note by {selectedNote.Author} from '{participant.Name}'");
        }

        public async Task ExloadeFromArcive(Participant participant)
        {
            participant.IsInArchive = false;
            await UpdateParticipaint(participant)
                .ConfigureAwait(false);
            _logger.LogInformation($"Exloade participaint {participant.Id} from archiv");
            
        }

        public Task<IsraelParticipant> GetIsraeliParticipantById(string id)
        {
            return _unitOfWork.IsraelParticipantsRepositry.GetByIdAsync(id);
        }

        public Task<WorldParticipant> GetWolrdParticipantById(string id)
        {
            return _unitOfWork.WorldParticipantsRepositry.GetByIdAsync(id);
        }
    }
}
