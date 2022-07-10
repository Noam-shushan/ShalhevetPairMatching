using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using PairMatching.Models.Dtos;
using PairMatching.Configurations;
using Newtonsoft.Json;
using PairMatching.Tools;

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
                .WixApi["newApplicants"]
                .Replace("{INDEX OF THE LAST APPLICANTS RECIEVED}", $"{index}");

            var jsonContent = await _http.GetAsync(query);

            var parsedObject = JObject.Parse(jsonContent);

            var jsonPartsDtos = parsedObject["items"].ToString();

            var data = JsonConvert
                .DeserializeObject<IEnumerable<ParticipantWixDto>>(jsonPartsDtos);

            return data;
        }

        public async Task SendEmail(EmailWixDto email)
        {
            var query = _configuration.WixApi["sendEmails"];

            var body = JsonConvert.SerializeObject(email, Formatting.Indented);

            await _http.PostAsync(query, body);
        }

        public async Task<ParticipantWixDto> GetOneParticipant(int index)
        {
            var query = _configuration
                .WixApi["getApplicant"]
                .Replace("{INDEX OF THE APPLICANT TO GET INFO}", $"{index}");

            var content = await _http.GetAsync(query);

            var data = JsonConvert.DeserializeObject<ParticipantWixDto>(content);

            return data;
        }

        public async Task NewPair(NewPairWixDto pairDto)
        {
            var query = _configuration.WixApi["sendMembers"];

            var body = JsonConvert.SerializeObject(pairDto, Formatting.Indented);

            await _http.PostAsync(query, body);
        }

        public async Task UpsertParticipaint(UpsertParticipantOnWixDto participantDto)
        {
            var query = _configuration.WixApi["updateMember"];
            var dto = new UpsertParticipantOnWixDtoNoId();
            string body;
            if (participantDto._id == "")
            {
                participantDto.isNew = true;
                participantDto.CopyPropertiesTo(dto);
                body = JsonConvert.SerializeObject(dto, Formatting.Indented);
            }
            else
            {
                body = JsonConvert.SerializeObject(participantDto, Formatting.Indented);
            }
           
            await _http.PostAsync(query, body);
        }

    }
}
