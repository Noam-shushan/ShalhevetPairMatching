using PairMatching.Configurations;
using static PairMatching.Tools.HelperFunction;
using PairMatching.DomainModel.DataAccessFactory;
using Prism.Ioc;
using System;
using Newtonsoft.Json;

namespace PairMatching.Root
{
    public class Startup
    {
        MyConfiguration _configuration;
        
        public Startup()
        {            
        }

        public MyConfiguration GetConfigurations()
        {
            return _configuration ??= Get();

            static MyConfiguration Get()
            {
                var jsonString = ReadJson(@"Resources/appsetting.json");
                var configurations = JsonConvert.DeserializeObject<MyConfiguration>(jsonString);
                return configurations ?? throw new Exception("No Configurations");
            }
        }
    }
}
