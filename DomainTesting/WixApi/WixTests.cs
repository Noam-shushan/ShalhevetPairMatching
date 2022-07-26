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

namespace DomainTesting.WixApi
{
    [TestFixture]
    public class WixTests
    {
        readonly WixDataReader _wix;
        
        public WixTests()
        {
            var conf = GetConfigurations();
            _wix = new WixDataReader(conf);
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        private MyConfiguration GetConfigurations()
        {
            var jsonString = ReadJson(@"C:\Users\Asuspcc\source\Repos\ShalhevetPairMatching\GuiWpf\Resources\appsetting.json");
            var configurations = JsonConvert.DeserializeObject<MyConfiguration>(jsonString);
            return configurations ?? throw new Exception("No Configurations");
        }

        [Test]
        public void UpdateMembers()
        {
            
        }

        [Test]
        public void NewPairToWixTest()
        {         
            //await _wix.NewPair(new NewPairWixDto
            //{
            //    chevrutaIdFirst = "8f60532a-de89-427a-b628-ede463dcf093",
            //    chevrutaIdSecond = "e1bc6d5e-9913-4e2b-9fae-26e42bfcc556",
            //    date = DateTime.Now,
            //    trackId = PrefferdTracks.Payer.GetDescriptionIdFromEnum(),
            //});
        }
    }
}
