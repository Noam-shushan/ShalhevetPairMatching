using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using PairMatching.Models.Dtos;
using PairMatching.Configurations;

namespace PairMatching.WixApi
{
    public class WixDataReader
    {
        readonly RestHttp _http = new();

        readonly MyConfiguration _configuration;

        public WixDataReader(MyConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<ParticipantWixDto>> GetNewParticipants(int index = 100)
        {
            var query = _configuration
                .WixApi["GET"]
                .Replace("{INDEX OF THE LAST APPLICANTS RECIEVED}", $"{index}");

            var content = await _http.GetAsync(query);

            var data = JsonConvert.DeserializeObject<IEnumerable<ParticipantWixDto>>(content);

            return data;
        }

        public async Task<ParticipantWixDto> GetOneParticipant(int index)
        {
            var query = _configuration
                .WixApi["GET_ONE"]
                .Replace("{INDEX OF THE APPLICANT TO GET INFO}", $"{index}");

            var content = await _http.GetAsync(query);

            var data = JsonConvert.DeserializeObject<ParticipantWixDto>(content);

            return data;
        }

        public async Task PostToWix(ParticipantWixDto participantDto) 
        {
            var query = _configuration.WixApi["POST"];

            var body = JsonConvert.SerializeObject(participantDto);

            await _http.PostWithRestSharpAsync(query, body);
        }
    }
}
