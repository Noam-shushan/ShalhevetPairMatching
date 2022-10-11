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

namespace DomainTesting.DataAccess
{
    [TestFixture]
    public class DbTests
    {
        readonly IUnitOfWork _db;

        public DbTests()
        {
            var conf = new Startup()
                .GetConfigurations();
            _db = new UnitOfWork(conf);
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
    }
}
