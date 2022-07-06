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
                .IsraelParticipantsRepositry
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
                    .Select(p => p.ToParticipant())
                    .ToLookup(p => p.IsFromIsrael);

                await _unitOfWork.IsraelParticipantsRepositry
                    .Insert(list[true]
                    .Select(p => p as IsraelParticipant));

                await _unitOfWork.WorldParticipantsRepositry
                        .Insert(list[false]
                        .Select(p => p as WorldParticipant));

                config.WixIndex = partsDtos.Max(p => p.index);
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

        // TODO : finish her
        public async Task MoveToNewDatabaseTest()
        {
            var students = await _unitOfWork
                .StudentRepositry
                .GetAllAsync(s => !s.IsDeleted);

            var toPartsList = students
                .Select(s => s.ToParticipant())
                .ToLookup(p => p.IsFromIsrael);
            
            var israelParts = toPartsList[true].Select(p => p as IsraelParticipant);
            var worldParts = toPartsList[false].Select(p => p as WorldParticipant);
            
            var tasks = new List<Task>
            {
                _unitOfWork.IsraelParticipantsRepositry.Insert(israelParts),
                _unitOfWork.WorldParticipantsRepositry.Insert(worldParts)
            };
            await Task.WhenAll(tasks);
        }

        // TODO : finish her
        public async Task MoveToNewDatabaseWithMatching()
        {
            var students = await _unitOfWork
                .StudentRepositry
                .GetAllAsync(s => !s.IsDeleted);

            var toPartsList = students
                .Select(s => s.ToParticipant())
                .ToLookup(p => p.IsFromIsrael);

            var israelParts = toPartsList[true].Select(p => p as IsraelParticipant);
            var worldParts = toPartsList[false].Select(p => p as WorldParticipant);
            
            var tasks = new List<Task>();
            foreach (var p in israelParts)
            {
                var s = students.FirstOrDefault(s => s.Id == p.OldId);
                if (s != null)
                {
                    p.MatchTo = from wp in worldParts
                                where s.MatchTo.Contains(wp.OldId)
                                select wp.Id;
                }
                tasks.Add(_unitOfWork.IsraelParticipantsRepositry.Update(p));
            }
            foreach (var p in worldParts)
            {
                var s = students.FirstOrDefault(s => s.Id == p.OldId);
                if (s != null)
                {
                    p.MatchTo = from ip in israelParts
                                where s.MatchTo.Contains(ip.OldId)
                                select ip.Id;
                }
                tasks.Add(_unitOfWork.WorldParticipantsRepositry.Update(p));
            }
            await Task.WhenAll(tasks);
        }

        public async Task MoveOneToNewDatabaseTest()
        {
            //var student = await _unitOfWork
            //    .StudentRepositry
            //    .GetByIdAsync(4);

            //var toPart = student.ToParticipant();
            //if (toPart.IsFromIsrael)
            //{
            //    await _unitOfWork.IsraelParticipantsRepositry.Insert(toPart as IsraelParticipant);
            //}
            //else
            //{
            //    await _unitOfWork.WorldParticipantsRepositry.Insert(toPart as WorldParticipant);
            //}

            var p = await _unitOfWork
                .WorldParticipantsRepositry
                .GetByIdAsync("62c568ac83465d7fa9984c41");
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
