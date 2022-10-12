﻿using MongoDB.Bson;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public class ParticipantService : IParticipantService
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        readonly string _configId;

        public ParticipantService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();

            _wix = new WixDataReader(configuration);

            _configId = configuration.ConfigIdInMongo;
        }

        public async Task<IEnumerable<Participant>> GetAll()
        {
            var result = new List<Participant>();
            var tasks = new List<Task>();

            //await SetNewParticipintsFromWix();

            var ips = GetAllFromIsrael();
            var wps = GetAllFromWorld();

            //await _unitOfWork.IsraelParticipantsRepositry.SaveToDrive();
            //await _unitOfWork.WorldParticipantsRepositry.SaveToDrive();

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

            var partsDtos = await _wix.GetNewParticipants(index);

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

                await _unitOfWork.IsraelParticipantsRepositry
                    .InsertMany(ips);

                await _unitOfWork.WorldParticipantsRepositry
                        .InsertMany(wps);

                config.WixIndex = partsDtos.Max(p => p.index);
                await _unitOfWork.ConfigRepositry.UpdateDbConfig(config);
            }
        }

        public async Task<Participant> UpserteParticipant(Participant part)
        {
            dynamic wixId = await GetWixId(part);
            
            part.WixId = wixId;

            if (part is IsraelParticipant ip)
            {
                return await _unitOfWork
                .IsraelParticipantsRepositry
                .Insert(ip);
            }
            else if (part is WorldParticipant wp)
            {
                return await _unitOfWork
                .WorldParticipantsRepositry
                .Insert(wp);
            }

            return null;   
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
            if(_countryUtcs is null)
            {
                _countryUtcs = Init();
            }
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
    }
}
