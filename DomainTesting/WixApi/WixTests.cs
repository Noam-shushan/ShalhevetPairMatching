using System;
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
            var parts = await _wix.GetNewParticipants(122);
            
        }

        [Test]
        public async Task NewPairToWixTest()
        {
            var parts = await _wix.GetNewParticipants();
            
            var max = parts.Max(p => p.index);
            var orderd = parts.OrderByDescending(p => p.index);
            
            var a = orderd.FirstOrDefault();
            var b = orderd.FirstOrDefault(p => p.index == max - 1);

            await _wix.NewPair(new NewPairWixDto
            {
                chevrutaIdFirst = a.contactId,
                chevrutaIdSecond = b.contactId,
                date = DateTime.Now,
                trackId = "df6ce1e8-1839-4749-bd4f-495295d75657"
            });
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

        [Test]
        public async Task SetWixIdForIsraelParticipaints()
        {
            var ips = await _db.IsraelParticipantsRepositry
                .GetAllAsync(p => string.IsNullOrEmpty(p.WixId) && p.Email != "אין" && p.Email != "@" && p.Email != "");

            var tasks = new List<Task>();
            
            int partsCount = ips.Count();
            int idCount = 0;
            foreach (var ip in ips)
            {
                dynamic id = "";
                try
                {
                    id = await GetWixId(ip);
                }
                catch (Exception)
                {
                    continue;
                }
                if (id is string i)
                {
                    idCount++;
                    ip.WixId = i;
                    tasks.Add(_db.IsraelParticipantsRepositry
                    .Update(ip));
                }
            }
            await Task.WhenAll(tasks);
            Assert.AreEqual(idCount, partsCount);
        }

        [Test]
        public async Task SetWixIdForWorldParticipaints()
        {
            var wps = await _db.WorldParticipantsRepositry
                .GetAllAsync(p => string.IsNullOrEmpty(p.WixId));

            var tasks = new List<Task>();

            int partsCount = wps.Count();
            int idCount = 0;
            foreach (var wp in wps)
            {
                dynamic id = "";
                try
                {
                    id = await GetWixId(wp);
                }
                catch (Exception)
                {
                    continue;
                }
                if (id is string i)
                {
                    idCount++;
                    wp.WixId = i;
                    tasks.Add(_db.WorldParticipantsRepositry
                    .Update(wp));
                }
            }
            await Task.WhenAll(tasks);
            Assert.AreEqual(idCount, partsCount);
        }

        [Test]
        public async Task ClearWixId()
        {
            var tasks = new List<Task>();
            
            var ips = await _db.IsraelParticipantsRepositry.GetAllAsync();
            foreach (var ip in ips)
            {
                ip.WixId = "";
                tasks.Add(_db.IsraelParticipantsRepositry.Update(ip));
            }
            
            var wps = await _db.WorldParticipantsRepositry.GetAllAsync();
            
            foreach (var wp in wps)
            {
                wp.WixId = "";
                tasks.Add(_db.WorldParticipantsRepositry.Update(wp));
            }
            
            await Task.WhenAll(tasks);
        }
    }
}
