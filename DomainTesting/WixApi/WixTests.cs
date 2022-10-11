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

namespace DomainTesting.WixApi
{
    [TestFixture]
    public class WixTests
    {
        readonly WixDataReader _wix;

        readonly IUnitOfWork _db;

        public WixTests()
        {
            var conf = new Startup() 
                .GetConfigurations();
            _db = new UnitOfWork(conf);
            _wix = new WixDataReader(conf);
            //AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        [Test]
        public void UpdateMembers()
        {
            
        }

        [Test]
        public async Task NewPairToWixTest()
        {
            var parts = await _wix.GetNewParticipants();
            
            var max = parts.Max(p => p.index);
            var orderd = parts.OrderByDescending(p => p.index);
            
            var a = orderd.FirstOrDefault();
            var b = orderd.FirstOrDefault(p => p.index == max - 1);
        }

        [Test]
        public async Task SendEmailFromWix()
        {
            var email = new
            {
                to = new string[] { "494c66f2-7329-4b1d-aebf-3afac3662148", "bc55f01b-c32d-487c-bde8-0fe84992c394" },
                subject = "Subject",
                body = "Body",
                link = ""
            };
            await _wix.SendEmail(email);
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
                .GetAllAsync(p => string.IsNullOrEmpty(p.WixId));

            int partsCount = ips.Count();
            int idCount = 0;
            foreach (var ip in ips)
            {
                var id = await GetWixId(ip);
                if (id is string i)
                {
                    idCount++;
                    ip.WixId = i;
                    await _db.IsraelParticipantsRepositry
                    .Update(ip);
                }
            }
            Assert.AreEqual(idCount, partsCount);
        }

        [Test]
        public async Task SetWixIdForWorldParticipaints()
        {
            var wps = await _db.WorldParticipantsRepositry
                .GetAllAsync(p => string.IsNullOrEmpty(p.WixId));

            int partsCount = wps.Count();
            int idCount = 0;
            foreach (var wp in wps)
            {
                var id = await GetWixId(wp);
                if(id is string i)
                {
                    idCount++;
                    wp.WixId = i;
                    await _db.WorldParticipantsRepositry
                    .Update(wp);
                }
            }
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
