using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWorks;
using PairMatching.Models;
using Newtonsoft.Json;
using PairMatching.Models.Dtos;
using PairMatching.Tools;
using PairMatching.WixApi;
using System.Security.Cryptography;
using System.Xml.Linq;
using PairMatching.Root;
using PairMatching.GoogleSheet;
using System.Drawing;

namespace DomainTesting.DataAccess
{
    [TestFixture]
    public class DbTests
    {
        readonly IUnitOfWork _db;

        readonly MyConfiguration _conf;

        readonly WixDataReader _wix;

        public DbTests()
        {
            _conf = new Startup()
                .GetConfigurations();
            _db = new UnitOfWork(_conf);
            _wix = new WixDataReader(_conf);
        }

        [Test]
        public async Task InsertNewItemToMongo()
        {
            var part = new IsraelParticipant
            {
                Country = "blabla",
                DateOfRegistered = DateTime.Now,
                Email = "blabla",
                Name = "test",
            };
            var id = await _db.IsraelParticipantsRepositry.Insert(part);
        }

        [Test]
        public async Task AddProperty()
        {
            var pairs = await _db.EmailRepositry.GetAllAsync();
        }

        [Test]
        public async Task SaveToDrive()
        {
            var tasks = new List<Task>
            {
                _db.IsraelParticipantsRepositry.SaveToDrive(),
                _db.WorldParticipantsRepositry.SaveToDrive(),
                _db.EmailRepositry.SaveToDrive(),
                _db.PairsRepositry.SaveToDrive()
            };

            await Task.WhenAll(tasks);
        }

        [Test]
        public async Task FixDbAndGoogleSheet()
        {       
            var missingFromWd = new string[] 
            { 
                "rob@boundlessliving.org", 
                "tamibenayon@yahoo.ca", 
                "Scotthertzberg68@gmail.com",
                "drmindybethlipson@gmail.com"
            };
            var parser = new GoogleSheetParser();
            var lastWolrdRow = await parser.ReadAsync(new EnglishDiscriptor(new SpredsheetLastRange
            {
                HebrewSheets = "A141:Z",
                EnglishSheets = "A123:Z"
            }, _conf));

            if (parser.NewStudents.Any())
            {
                var parts = parser.NewStudents
                    .Select(s => s.ToParticipant())
                    .ToLookup(p => p is IsraelParticipant);

                var worlds = parts[false].Select(p => p as WorldParticipant);

                foreach(var wp in worlds)
                {
                    //if (missingFromWd.Contains(wp.Email))
                    //{
                    //    var partWixDto = wp.ToWorldParticipantWixDto();
                    //    var id = await _wix.NewParticipant(partWixDto);
                    //    if(id is not null)
                    //    {
                    //        wp.WixId = id.ToString();
                    //    }
                    //}
                }
                await _db.WorldParticipantsRepositry.InsertMany(worlds);
            }


        }
    }
}
