using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using PairMatching.Models.Dtos;
using PairMatching.Configurations;
using Newtonsoft.Json;
using PairMatching.Tools;
using PairMatching.Models;
using System.Net.Http.Json;
using System.IO;

namespace PairMatching.WixApi
{
    public class WixDataReader
    {
        readonly RestHttp _http = new();

        public static int MinIndex = 100;

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

        public async Task<IEnumerable<EmailAddress>> SendEmail(EmailModel emailDto)
        {
            var query = _configuration.WixApi["sendEmails"];
            
            var email = new
            {
                to = emailDto.To.Select(i => i.ParticipantWixId),
                subject = emailDto.Subject,
                body = emailDto.Body
            };        

            await _http.PostAsync(query, email);

            return null;
        }

        public async Task<string> SendEmail(dynamic email)
        {
            var query = _configuration.WixApi["sendEmails"];

            var jsonContent = await _http.PostAsync(query, email);

            var parsedObject = JObject.Parse(jsonContent);

            var id = parsedObject["inserted"]["_id"];

            return id?.ToString();
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

            await _http.PostAsync(query, pairDto);
        }

        public async Task UpsertParticipaint(UpsertParticipantOnWixDto participantDto)
        {
            var query = _configuration.WixApi["updateMember"];
            var dto = new UpsertParticipantOnWixDtoNoId();
            if (participantDto._id == "")
            {
                participantDto.isNew = true;
                participantDto.CopyPropertiesTo(dto);                
            }
           
            await _http.PostAsync(query, participantDto);
        }

        public async Task<string> NewParticipant(dynamic participantWixDto)
        {
            var query = _configuration.WixApi["newMember"];
            
            // "inserted" : {}
            var jsonContent = await _http.PostAsync(query, participantWixDto);

            var parsedObject = JObject.Parse(jsonContent);

            var id = parsedObject["inserted"]["contactId"];

            return id?.ToString();
        }

        public async Task<IEnumerable<EmailRecipientsWixDto>> VerifieyEmail(string emailId)
        {
            var query = _configuration.WixApi["verifieyEmail"]
                .Replace("{Email Id}", emailId);

            var content = await _http.GetAsync(query);

            var data = JsonConvert.DeserializeObject<IEnumerable<EmailRecipientsWixDto>>(content);

            return data;
        }
    }
}
