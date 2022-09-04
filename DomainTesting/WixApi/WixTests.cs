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

namespace DomainTesting.WixApi
{
    [TestFixture]
    public class WixTests
    {
        readonly WixDataReader _wix;
        
        public WixTests()
        {
            var conf = new Startup() 
                .GetConfigurations();
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

        public async Task SendEmailFromWix()
        {
            var email = new
            {
                to = "",
                subject = "Subject",
                body = "Body"
            };
            await _wix.SendEmail(email);
        }

        
    }
}
