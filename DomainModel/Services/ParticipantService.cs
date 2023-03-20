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

            await SetNewParticipintsFromWix();

            var ips = GetAllFromIsrael();
            var wps = GetAllFromWorld();

            tasks.Add(ips);
            tasks.Add(wps);

            await Task.WhenAll(tasks);

            result.AddRange(ips.Result);
            result.AddRange(wps.Result);

            return result;
        }

        public async Task<IEnumerable<IsraelParticipant>> GetAllFromIsrael()
        {
            return await _unitOfWork
                .IsraelParticipantsRepositry
                .GetAllAsync(ip => !ip.IsDeleted);
        }

        public async Task<IEnumerable<WorldParticipant>> GetAllFromWorld()
        {
            return await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync(ip => !ip.IsDeleted);
        }

        public async Task SetNewParticipintsFromWix()
        {
            var config = await _unitOfWork.ConfigRepositry
            .GetDbConfig(_configId);

            var max = config?.WixIndex;
            int temp = max ?? WixDataReader.MinIndex;
            int index = temp < WixDataReader.MinIndex ? WixDataReader.MinIndex : temp;

            IEnumerable<ParticipantWixDto> partsDtos;
            try
            {
                partsDtos = await _wix.GetNewParticipants(index);
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

                var countryUts = GetCountryUtcs()
                    .ToDictionary(c => c.Country, c => c.UtcOffset);

                foreach (var wp in wps)
                {
                    wp.UtcOffset = countryUts[wp.Country];
                }
                
                if (ips.Any())
                {
                    await _unitOfWork.IsraelParticipantsRepositry
                            .InsertMany(ips);
                }

                if (wps.Any())
                {
                    await _unitOfWork.WorldParticipantsRepositry
                            .InsertMany(wps);
                }
                
                var names = partsDtos.Select(p => p.fullName);
                _logger.LogInformation($"New members added: {string.Join(", ", names)}, count: {partsDtos.Count()}");
                
                int newMaxIndex = partsDtos.Max(p => p.index);
                if(newMaxIndex != config.WixIndex)
                {
                    config.WixIndex = newMaxIndex;
                    await _unitOfWork.ConfigRepositry
                        .UpdateDbConfig(config);
                }
            }
        }

        public async Task AddNote(Note note, Participant participant)
        {
            participant.Notes.Add(note);
            await UpdateParticipaint(participant);
            _logger.LogInformation($"Note added to participant: {participant.Id} by {note.Author}");
        }

        public async Task UpdateParticipaint(Participant participant)
        {
            try
            {
                if (participant is IsraelParticipant ip)
                {
                    await _unitOfWork
                        .IsraelParticipantsRepositry
                        .Update(ip);
                }
                else if (participant is WorldParticipant wp)
                {
                    await _unitOfWork
                        .WorldParticipantsRepositry
                        .Update(wp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not update member '{participant.Id}'", ex);
                throw new UserException($"Sory... there is some error\nDo not worry the develper is informed");
            }
        }

        public async Task<Participant> InsertParticipant(Participant part)
        {
            dynamic wixId = ""; 
            try
            {
                wixId = await GetWixId(part);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while trying to add '{part.Name}' to wix", ex);
                throw new UserException($"Error while trying to add '{part.Name}' to wix");
            }

            part.WixId = wixId.contactId;
            part._WixId = wixId._id;

            Participant result = new();

            try
            {
                if (part is IsraelParticipant ip)
                {
                    result = await _unitOfWork
                    .IsraelParticipantsRepositry
                    .Insert(ip);
                }
                else if (part is WorldParticipant wp)
                {
                    result = await _unitOfWork
                    .WorldParticipantsRepositry
                    .Insert(wp);
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
            await RemoveMatchParticipaints(participant);

            participant.IsDeleted = true;

            await UpdateParticipaint(participant);

            _logger.LogInformation($"Delete participaint {participant.Id}");
        }

        public async Task SendToArcive(Participant participant)
        {
            await RemoveMatchParticipaints(participant);
            
            participant.IsInArchive = true;

            await UpdateParticipaint(participant);

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
                            .GetByIdAsync(matchParticipant);
                    }
                    else
                    {
                        p = await _unitOfWork
                            .IsraelParticipantsRepositry
                         .GetByIdAsync(matchParticipant);
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
                await Task.WhenAll(tasks);
                
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
                var jsonString = HelperFunction.ReadJson(@"Resources\Countries.json");
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
            await UpdateParticipaint(participant);
            _logger.LogInformation($"Delete note by {selectedNote.Author} from '{participant.Name}'");
        }
    }
}
