using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Models;
using PairMatching.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.WixApi;
using PairMatching.Configurations;
using System.IO;
using Newtonsoft.Json;
using PairMatching.Tools;
using static PairMatching.Tools.HelperFunction;

namespace PairMatching.DomainModel.Services
{
    public class ParticipantService : IParticipantService
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        public ParticipantService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();

            _wix = new WixDataReader(configuration);
        }

        public async Task<IEnumerable<Participant>> GetAllParticipants()
        {
            await ReadNewFromWix();

            return await _unitOfWork
                .ParticipantsRepositry
                .GetAllAsync();

        }

        private async Task ReadNewFromWix()
        {
            var configColl = await _unitOfWork.ConfigRepositry
                                    .GetMaxIndexOfWixData();

            var config = configColl.FirstOrDefault();

            var max = config?.WixIndex;
            int temp = max ?? 100;
            int index = temp < 100 ? 100 : temp;

            var partsDtos = await _wix.GetNewParticipants(index);

            if (partsDtos.Any())
            {
                var list = partsDtos
                    .Select(p => p.ToParticipant());

                await _unitOfWork.ParticipantsRepositry
                    .Insert(list);

                config.WixIndex = list.Max(x => x.WixIndex);
                await _unitOfWork.ConfigRepositry.UpdateDbConfig(config);
            }
        }

        public async Task UpdateParticipant(Participant participant)
        {
            await _wix.PostToWix(new UpdateParticipantOnWixDto
            {
                _id = participant.WixId,
                email = participant.Email,
                fullName = participant.Name,
                chevrutaId = "",
                preferredTrack = participant
                .PairPreferences
                .Tracks
                .Select(t => t.GetDescriptionFromEnumValue()).ToList(),
                tel = participant.PhoneNumber
            });
        }

        public async Task<IEnumerable<Participant>> GetParticipantsWix()
        {
            var parts = await _wix.GetNewParticipants();
            var list = parts.Select(p => p.ToParticipant());
            return list;
        }

        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            var result = await _unitOfWork
                     .StudentRepositry
                .GetAllAsync(s => !s.IsDeleted);
            return result
                .Take(50);
        }

        public IEnumerable<CountryUtc> GetCountryUtcs()
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

    public class CountryUtc
    {
        public string Country { get; set; }

        public string UtcTimeOffset { get; set; }

        public TimeSpan UtcOffset { get; set; }
    }
}
