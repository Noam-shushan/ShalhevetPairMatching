using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace PairMatching.WixApi
{
    public class RestHttp
    {
        public async Task PostAsync(string url, object body)
        {
            try
            {
                using var client = new RestClient(url);

                var request = new RestRequest();
                
                request.AddJsonBody(body);

                var respons = await client.PostAsync(request);
                if (!respons.IsSuccessful)
                {
                    throw new Exception(respons.Content);
                }
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
                using var client = new HttpClient();
                
                var response = await client.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }
    }
}
