﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PairMatching.WixApi;
using PairMatching.Tools;
using PairMatching.Models;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Configurations;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PairMatching.Models.Dtos;
using PairMatching.Root;
using PairMatching.DataAccess.UnitOfWorks;
using static MongoDB.Driver.WriteConcern;
using Microsoft.Extensions.Configuration;

namespace DomainTesting.WixApi
{
    [TestFixture]
    public class WixTests
    {
        readonly WixDataReader _wix;
        private readonly string _configId;
        readonly IUnitOfWork _db;

        public WixTests()
        {
            var conf = new Startup() 
                .GetConfigurations();
            _db = new UnitOfWork(conf);
            _wix = new WixDataReader(conf);
            _configId = conf.ConfigIdInMongo;
            //AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        [Test]
        public void UpdateMembers()
        {
            
        }

        [Test]
        public async Task GetAll()
        {
            //var partsDto = await _wix.GetNewParticipants(0);
            //var wps = from p in partsDto
            //          where p is WorldParticipantWixDto
            //          select (p as WorldParticipantWixDto).ToWorldParticipant();
            //var ips = from p in partsDto
            //          where p is IsraelParticipantWixDto
            //          select (p as IsraelParticipantWixDto).ToIsraelParticipant();

            var noamDto = await _wix.GetOneParticipant(185);
            var noam = (noamDto as IsraelParticipantWixDto).ToIsraelParticipant();
            var noamInDb = 
                await _db.IsraelParticipantsRepositry.GetByIdAsync("64a26e894c735c0dc0dfc181");
            noamInDb.WixId = noam.WixId;
            noamInDb._WixId = noam._WixId;
            await _db.IsraelParticipantsRepositry
                .Update(noamInDb);
        }

        [Test]
        public async Task NewPairToWixTest()
        {
            var id = await _wix.NewPair(new NewPairWixDto
            {
                chevrutaIdFirst = "816b10ef-0076-4d6a-b51d-a441f873dcfc",
                chevrutaIdSecond = "0356523c-4c8f-47eb-9429-7d2e6671ae56",
                date = DateTime.Now,
                trackId = "df6ce1e8-1839-4749-bd4f-495295d75657"
            });
        }

        [Test]
        public async Task VerfiyNewPair()
        {
            // "11d4ccb4-1ff5-4a1b-b459-767af3c02b6e"
            var data = await _wix.VerifieyNewPair("11d4ccb4-1ff5-4a1b-b459-767af3c02b6e");
        }

        [Test]
        public async Task SendEmailFromWix()
        {        
            var parts = await _wix.GetNewParticipants();
            // "0356523c-4c8f-47eb-9429-7d2e6671ae56"
            var max = parts.Max(p => p.index);
            var orderd = parts.OrderByDescending(p => p.index);

            var a = orderd.FirstOrDefault();
            var b = orderd.FirstOrDefault(p => p.index == max - 1);

            var email = new
            {
                to = new string[] { a.contactId, b.contactId },
                subject = "נושא",
                body = "תוכן",
                htmlBody = "",
                hasHtmlBody = false,
                link = @"https://www.ynet.co.il/PicServer4/2016/10/27/7346289/734627233624799801423yes2130.jpg",
                language = "he"
            };
            await _wix.SendEmail(email);
        }

        [Test]
        public async Task VerfiyEmails()
        {
            // {65a2c272-516b-4413-a89f-728206eeb37f}
            var data = await _wix.VerifieyEmail("65a2c272-516b-4413-a89f-728206eeb37f");
        }

        private async Task<WixIdDto> GetWixId(Participant part)
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

            var wixId = await _wix.NewParticipant(partDto) as WixIdDto;
            return wixId;
        }

        [Test]
        public async Task SetWixIdForIsraelParticipaints()
        {
            var ips = await _db.IsraelParticipantsRepositry
                .GetAllAsync(p => string.IsNullOrEmpty(p._WixId) && p.Email != "אין" && p.Email != "@" && p.Email != "");

            var tasks = new List<Task>();
            
            int partsCount = ips.Count();
            int idCount = 0;
            foreach (var ip in ips)
            {
                WixIdDto id = null;
                try
                {
                    id = await GetWixId(ip);
                }
                catch (Exception)
                {
                    continue;
                }
                
                idCount++;
                ip.WixId = id.Id;
                ip._WixId = id._Id;
                tasks.Add(_db.IsraelParticipantsRepositry
                .Update(ip));
            }
            await Task.WhenAll(tasks);
            Assert.AreEqual(idCount, partsCount);
        }

        [Test]
        public async Task SetWixIdForWorldParticipaints()
        {
            var wps = await _db.WorldParticipantsRepositry
                .GetAllAsync(p => string.IsNullOrEmpty(p._WixId));

            var tasks = new List<Task>();

            int partsCount = wps.Count();
            int idCount = 0;
            foreach (var wp in wps)
            {
                WixIdDto id = null;
                try
                {
                    id = await GetWixId(wp);
                }
                catch (Exception)
                {
                    continue;
                }

                idCount++;
                wp.WixId = id.Id;
                wp._WixId = id._Id;
                tasks.Add(_db.WorldParticipantsRepositry
                .Update(wp));
            }
            await Task.WhenAll(tasks);
            Assert.AreEqual(idCount, partsCount);
        }

        [Test]
        public async Task FixWixId()
        {
            var part =
                await _db.WorldParticipantsRepositry.GetByIdAsync("6346b6609269558a546fc1ff");
            part._WixId = "b129d489-8895-46c0-b24d-32e3a68212cf";
            await _db.WorldParticipantsRepositry.Update(part);
        }
    }
}
