using PairMatching.Configurations;
using static PairMatching.Tools.HelperFunction;
using PairMatching.DomainModel.DataAccessFactory;
using Prism.Ioc;
using System;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using static System.Net.WebRequestMethods;
using System.Net.NetworkInformation;

namespace PairMatching.Root
{
    public class Startup
    {
        MyConfiguration _configuration;
        
        public Startup()
        {     
            
        }

        public bool IsConnectedToInternet()
        {
                string host = @"google.com";  
                bool result = false;
                var p = new Ping();
                try
                {
                    PingReply reply = p.Send(host, 3000);
                    if (reply.Status == IPStatus.Success)
                        return true;
                }
                catch { }
                return result;
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
