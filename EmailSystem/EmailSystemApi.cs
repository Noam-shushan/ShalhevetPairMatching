using System;
using System.Threading.Tasks;
using PairMatching.Configurations;
using RestSharp;
using System.Linq;
using System.Collections.Generic;


namespace EmailSystem
{
    public class EmailSystemApi
    {
        readonly MyConfiguration _config;
        
        public EmailSystemApi(MyConfiguration config)
        {
            _config = config;
        }

        public async Task  ConectToMailerlite()
        {
            try
            {
                var client = new RestClient("https://api.mailerlite.com/api/v2/campaigns");
                var request = new RestRequest();

                request.AddHeader("Accept", "application/json");
                request.AddHeader("X-MailerLite-ApiDocs", "true");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-MailerLite-ApiKey", _config.Mailerlite);
                request.AddParameter("application/json", "{\"type\":\"null\",\"name\":\"campaign name\",\"from\":\"Account's default sender\",\"from_name\":\"Account's default sender\",\"language\":\"Account's default language code\"}", ParameterType.RequestBody);

                var response = await client.PostAsync(request);
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        
    }
}
