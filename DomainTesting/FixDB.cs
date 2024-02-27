using Newtonsoft.Json;
using NUnit.Framework;
using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWorks;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Models;
using PairMatching.Root;
using PairMatching.Tools;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PairMatching.Loggin;
using PairMatching.Models.Dtos;
using System.Diagnostics;

namespace DomainTesting
{
    [TestFixture]
    public class FixDB
    {
        readonly IUnitOfWork _unitOfWork;

        readonly MyConfiguration _config;

        readonly Logger _logger;
        readonly WixDataReader _wix;

        public FixDB()
        {
            _config = new Startup()
                .GetConfigurations();
            var db = new DataAccessFactory(_config);
            _unitOfWork = db.GetDataAccess();
            _logger = new Logger(_config.ConnctionsStrings);
            _wix = new(_config);
        }

        IEnumerable<CountryUtc> GetUtcs()
        {
            var jsonString = HelperFunction.ReadJson(@"C:\Users\Asuspcc\source\ShalhevetPairMatching\DomainModel\Resources\Countries.json");
            var result = JsonConvert.DeserializeObject<IEnumerable<CountryUtc>>(jsonString);
            foreach (var country in result)
            {
                country.UtcOffset = country.UtcTimeOffset.ToTimeSpan();
            }
            return result;
        }

        [Test]
        public async Task FixOpenQastion()
        {
            var part = await _unitOfWork
                .IsraelParticipantsRepositry
                .GetByIdAsync("649297e33630fbf9641f56a2");

            if (part == null)
            {
                return;
            }
            // b7ed0236-6646-44cc-85a4-b85daaee3155
            // b7ed0236-6646-44cc-85a4-b85daaee3155
            var all = await _wix.GetNewParticipants(100);
            
            var participantWixDto = all.FirstOrDefault(p => p._id == part._WixId) as IsraelParticipantWixDto;

            part.OpenQuestions = new OpenQuestionsForIsrael
            {
                AdditionalInfo = participantWixDto.additionalInfo,
                BiographHeb = participantWixDto.biographHeb,
                PersonalTraits = participantWixDto.personalTraits,
                WhoIntroduced = participantWixDto.whoIntroduced,
                WhyJoinShalhevet = participantWixDto.whyJoinShalhevet
            };

            await _unitOfWork.IsraelParticipantsRepositry.Update(part);
        }

        [Test]
        public async Task AddPropertyToMongo()
        {
            var ips = await _unitOfWork
                .IsraelParticipantsRepositry
                .GetAllAsync();

            var wps = await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync();
        }

        [Test]
        public async Task FixTracksTest()
        {
            var participaints = new List<Participant>();

            var ips = await _unitOfWork.IsraelParticipantsRepositry.GetAllAsync();
            var wps = await _unitOfWork.WorldParticipantsRepositry.GetAllAsync();

            participaints.AddRange(wps);
            participaints.AddRange(ips);

            var tasks = new List<Task>();
            foreach (var p in participaints)
            {
                if(p.PairPreferences.Tracks.Count() != p.PairPreferences.Tracks.Distinct().Count())
                {
                    p.PairPreferences.Tracks = p.PairPreferences.Tracks.Distinct();
                    tasks.Add(p is WorldParticipant ?
                        _unitOfWork.WorldParticipantsRepositry.Update(p as WorldParticipant) :
                        _unitOfWork.IsraelParticipantsRepositry.Update(p as IsraelParticipant));
                }
            }
            Console.WriteLine(tasks.Count);
            await Task.WhenAll(tasks);
        }

        [Test]
        public async Task GetLogs()
        {
            var logs = await _logger.GetErrorLogs();

            var mylogs = logs
                .OrderByDescending(l => l.Date);

            foreach(var log in mylogs)
            {
                Console.WriteLine(log);
            }
        }

        [Test]
        public async Task FixLearningTimeTest()
        {
            var participaints = new List<Participant>();

            var ips = await _unitOfWork.IsraelParticipantsRepositry.GetAllAsync();
            var wps = await _unitOfWork.WorldParticipantsRepositry.GetAllAsync();

            participaints.AddRange(wps);
            participaints.AddRange(ips);

            await Task.WhenAll(FixLearningTime(participaints));
        }

        public List<Task> FixLearningTime(IEnumerable<Participant> participants)
        {
            var tasks = new List<Task>();
            foreach (var part in participants)
            {
                if (part.PairPreferences.LearningTime
                    .Any(lt => lt.TimeInDay.Count() != lt.TimeInDay.Distinct().Count()))
                {
                    part.PairPreferences.LearningTime = from lt in part.PairPreferences.LearningTime
                                                        group lt by lt.Day into day
                                                        select new LearningTime
                                                        {
                                                            Day = day.Key,
                                                            TimeInDay = day.SelectMany(t => t.TimeInDay).Distinct()
                                                        };
                    if (part is IsraelParticipant ip)
                    {
                        tasks.Add(_unitOfWork.IsraelParticipantsRepositry.Update(ip));
                    }
                    else if (part is WorldParticipant wp)
                    {
                        tasks.Add(_unitOfWork.WorldParticipantsRepositry.Update(wp));
                    }
                }
            }
            Debug.WriteLine(tasks.Count);
            return tasks;
        }



        [Test]
        public async Task FixUtc()
        {
            var utcs = GetUtcs().ToList();

            var worlds = await _unitOfWork.WorldParticipantsRepositry.GetAllAsync();

            var tasks = new List<Task>();

            foreach (var w in worlds)
            {

                var utc = utcs.Find(uc => CompereOnlyLetters(uc.Country, w.Country));
                if (utc != null && utc.UtcOffset != w.UtcOffset)
                {
                    w.UtcOffset = utc.UtcOffset;
                    tasks.Add(_unitOfWork.WorldParticipantsRepositry.Update(w));
                }
            }

            await Task.WhenAll(tasks);
        }

        public bool CompereOnlyLetters(string frist, string second)
        {
            string s1 = RemoveAllCharsExeptLettersAndSpace(frist);
            string s2 = RemoveAllCharsExeptLettersAndSpace(second);
            return s1.ToLower() == s2.ToLower();
        }

        public string RemoveAllCharsExeptLettersAndSpace(string str)
        {
            var result = Regex.Replace(str, "[^0-9A-Za-z ,]", "");
            var onlyOneSpace = new Regex(@"[ ]{2,}", RegexOptions.None);
            result = onlyOneSpace.Replace(result, @" "); // "words with multiple spaces"

            return result.Trim();
        }
    }
}
