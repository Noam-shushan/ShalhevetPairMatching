﻿using Newtonsoft.Json;
using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Models;
using PairMatching.Models.Dtos;
using PairMatching.Tools;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Participant>> GetAll()
        {
            var result = new List<Participant>();
            var tasks = new List<Task>();
            
            var ips = GetAllFromIsrael();
            //var wps = GetAllFromWorld();

            tasks.Add(ips);
            //tasks.Add(wps);
            
            await Task.WhenAll(tasks);
            tasks.Clear();

            result.AddRange(ips.Result);

            var removeLearningTime = from p in ips.Result
                                     where !p.PairPreferences
                                     .LearningTime
                                     .Select(t => t.TimeInDay)
                                     .Any()
                                     select p;
            foreach (var p in removeLearningTime)
            {
                p.PairPreferences.LearningTime = p.PairPreferences.LearningTime.Where(t => t.TimeInDay.Any());
                if (p.PairPreferences.LearningTime.Any())
                {
                    tasks.Add(_unitOfWork.IsraelParticipantsRepositry
    .                           Update(p));
                }

            }
            await Task.WhenAll(tasks);            

            //result.AddRange(wps.Result);

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

        public async Task SetNewParticipints()
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

        public async Task UpserteParticipant(Participant part)
        {
            UpsertParticipantOnWixDto partDto = part.ToUpsertWixDto();
            await _wix.UpsertParticipaint(partDto);
        }

        public async Task<IEnumerable<Participant>> GetParticipantsWix()
        {
            var parts = await _wix.GetNewParticipants();
            var list = parts.Select(p => p.ToParticipant());
            return list;
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
