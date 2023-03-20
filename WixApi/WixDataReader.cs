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
using MongoDB.Bson.IO;
using JsonConvert = Newtonsoft.Json.JsonConvert;

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
            try
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
            catch (KeyNotFoundException)
            {
                return Enumerable.Empty<ParticipantWixDto>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> SendEmail(dynamic email)
        {
            try
            {
                var query = _configuration.WixApi["sendEmails"];

                var jsonContent = await _http.PostAsync(query, email);

                var parsedObject = JObject.Parse(jsonContent);

                var id = parsedObject["inserted"]["_id"];

                return id?.ToString();
            }
            catch (Exception)
            {

                throw;
            }
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

        public async Task<string> NewPair(NewPairWixDto pairDto)
        {
            var query = _configuration.WixApi["sendMembers"];

            var jsonContent = await _http.PostAsync(query, pairDto);

            var parsedObject = JObject.Parse(jsonContent);

            return parsedObject["id"].ToString();
        }

        public async Task<dynamic> NewParticipant(dynamic participantWixDto)
        {
            var query = _configuration.WixApi["newMember"];
            
            // "inserted" : {}
            var jsonContent = await _http.PostAsync(query, participantWixDto);

            var parsedObject = JObject.Parse(jsonContent);

            var _id = parsedObject["inserted"]["_id"];

            var contactId = parsedObject["inserted"]["contactId"];

            return new { _id, contactId };
        }

        public async Task<IEnumerable<EmailRecipientsWixDto>> VerifieyNewPair(string id)
        {
            try
            {
                var query = _configuration.WixApi["hevrutaEmailStatus"]
                        .Replace("{PAIR ID}", id);

                var jsonContent = await _http.GetAsync(query);

                var parsedObject = JObject.Parse(jsonContent);

                var content = parsedObject["status"].ToString();

                var data = JsonConvert.DeserializeObject<IEnumerable<EmailRecipientsWixDto>>(content);

                return data;
            }
            catch (KeyNotFoundException)
            {
                return Enumerable.Empty<EmailRecipientsWixDto>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<EmailRecipientsWixDto>> VerifieyEmail(string emailId)
        {
            try
            {
                var query = _configuration.WixApi["verifieyEmail"]
                    .Replace("{EMAIL ID}", emailId);

                var jsonContent = await _http.GetAsync(query);
                var parsedObject = JObject.Parse(jsonContent);
                var content = parsedObject["item"].ToString();

                var data = JsonConvert.DeserializeObject<IEnumerable<EmailRecipientsWixDto>>(content);

                return data;
            }
            catch (KeyNotFoundException)
            {
                return Enumerable.Empty<EmailRecipientsWixDto>();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
