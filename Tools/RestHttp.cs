using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Tools
{
    public class RestHttp
    {
        public async Task<string> PostAsync(string url, object body)
        {
            try
            {
                using var client = new RestClient(url);

                var request = new RestRequest();

                request.AddJsonBody(body);
                
                var response = await client.PostAsync(request);
                

                if (!response.IsSuccessful)
                {
                    throw new Exception(response.ErrorMessage);
                }

                return response.Content;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            try
            {
                using var client = new RestClient();

                var request = new RestRequest(new Uri(uri));

                var response = await client.GetAsync(request);
                // error: "No items found with index above 128"
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException(response.Content);
                }
                if (!response.IsSuccessful)
                {
                    throw new Exception(response.ErrorMessage);
                }
                return response.Content;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
