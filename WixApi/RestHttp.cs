using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;


namespace PairMatching.WixApi
{
    public class RestHttp
    {
        public async Task PostAsync(string uri, object data)
        {
            try
            {
                using var client = new HttpClient();

                var content = new StringContent(data.ToString());

                await client.PostAsync(uri, content);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task PostWithRestSharpAsync(string url, string body)
        {
            try
            {
                using var client = new RestClient(url);

                var request = new RestRequest("/resource/", Method.Post);

                request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;

                await client.PostAsync(request);
            }
            catch (System.Exception)
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
