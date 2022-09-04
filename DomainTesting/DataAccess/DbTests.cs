using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWork;
using PairMatching.Models;
using Newtonsoft.Json;
using PairMatching.Models.Dtos;
using PairMatching.Tools;
using PairMatching.WixApi;
using System.Security.Cryptography;
using System.Xml.Linq;
using PairMatching.Root;

namespace DomainTesting.DataAccess
{
    [TestFixture]
    public class DbTests
    {
        readonly IUnitOfWork _db;
        readonly WixDataReader _wix;

        public DbTests()
        {
            var conf = new Startup()
                .GetConfigurations();
            _db = new UnitOfWork(conf);
            _wix = new WixDataReader(conf);
        }  

        [Test]
        public async Task InsertNewItemToMongo()
        {
            var part = new IsraelParticipant
            {
                Country = "blabla",
                DateOfRegistered = DateTime.Now,
                Email = "blabla",
                Name = "test",
            };
            var id = await _db.IsraelParticipantsRepositry.Insert(part);
        }

        [Test]
        public async Task PostToWixTest()
        {
            var ips = await _db.IsraelParticipantsRepositry
                .GetAllAsync();

            var x = ips.FirstOrDefault();

            var y = ToIsDTO(x);

            await _wix.NewPart(y);
            
        }

        [Test]
        public async Task PostWorldToWixTest()
        {
            var wps = await _db.WorldParticipantsRepositry
                .GetAllAsync();

            var x = wps.FirstOrDefault();

            var y = ToWorldPart(x);

            await _wix.NewPart(y);

        }

        dynamic ToIsDTO(IsraelParticipant participant)
        {
            var name = participant.Name.Split();
            return new
            {
                additionalInfo = participant.OpenQuestions.AdditionalInfo,
                biographHeb = participant.OpenQuestions.BiographHeb,
                chevrotaSkills = participant.DesiredSkillLevel.GetDescriptionFromEnumValue(),
                email = participant.Email,
                gender = participant.Gender.GetDescriptionFromEnumValue(),
                levOfEn = participant.EnglishLevel.GetDescriptionFromEnumValue(),
                menOrWomen = participant.PairPreferences.Gender.GetDescriptionFromEnumValue(),
                preferredTrack = participant.PairPreferences.Tracks.Select(p => p.GetDescriptionFromEnumValue()).ToList(),
                learningStyle = participant.PairPreferences.LearningStyle.GetDescriptionFromEnumValue(),
                otherLan = participant.OtherLanguages,
                tel = participant.PhoneNumber,
                whoIntroduced = participant.OpenQuestions.WhoIntroduced,
                moreThanOneChevruta = participant.PairPreferences.NumberOfMatchs.ToString(),
                personalTraits = participant.OpenQuestions.PersonalTraits,
                whyJoinShalhevet = participant.OpenQuestions.WhyJoinShalhevet,
                sunday = ToLer(participant.PairPreferences
                .LearningTime.Where(l => l.Day == Days.Sunday)),
                monday = ToLer(participant.PairPreferences
                .LearningTime.Where(l => l.Day == Days.Monday)),
                thurseday = ToLer(participant.PairPreferences
                .LearningTime.Where(l => l.Day == Days.Thursday)),
                tuseday = ToLer(participant.PairPreferences
                .LearningTime.Where(l => l.Day == Days.Tuesday)),
                wednesday = ToLer(participant.PairPreferences
                .LearningTime.Where(l => l.Day == Days.Wednesday)),
                formLanguage = "Hebrew",
                fName = name.First(),
                lName = string.Join(" ", name.Skip(1))
            };
        }
            public dynamic ToWorldPart(WorldParticipant participant)
            {
                var name = participant.Name.Split();
                return new 
                {
                    additionalInfo = participant.OpenQuestions.AdditionalInfo,
                    anythingElse = participant.OpenQuestions.AnythingElse,
                    hopesExpectations = participant.OpenQuestions.HopesExpectations,
                    learningSkill = participant.SkillLevel.GetDescriptionFromEnumValue("eng"),
                    prefTra = participant.PairPreferences.Tracks.Select(t => t.GetDescriptionFromEnumValue("eng")).ToList(),
                    fName = name.First(),
                    lName = string.Join(" ", name.Skip(1)),
                    email = participant.Email,
                    gender = participant.Gender.GetDescriptionFromEnumValue("eng"),
                    menOrWomen = participant.PairPreferences.Gender.GetDescriptionFromEnumValue("eng"),
                    levOfEn = participant.DesiredEnglishLevel.GetDescriptionFromEnumValue("eng"),
                    utc = participant.UtcOffset.ToString(),                 
                    learningStyle = participant.PairPreferences.LearningStyle.GetDescriptionFromEnumValue("eng"),
                    otherLan = participant.OtherLanguages,
                    tel = participant.PhoneNumber,
                    whoIntroduced = participant.OpenQuestions.WhoIntroduced,
                    moreThanOneChevruta = participant.PairPreferences.NumberOfMatchs.ToString(),
                    sunday = ToLer(participant.PairPreferences
                    .LearningTime.Where(l => l.Day == Days.Sunday), "eng"),
                    monday = ToLer(participant.PairPreferences
                    .LearningTime.Where(l => l.Day == Days.Monday), "eng"),
                    thurseday = ToLer(participant.PairPreferences
                    .LearningTime.Where(l => l.Day == Days.Thursday), "eng"),
                    tuseday = ToLer(participant.PairPreferences
                    .LearningTime.Where(l => l.Day == Days.Tuesday), "eng"),
                    wednesday = ToLer(participant.PairPreferences
                    .LearningTime.Where(l => l.Day == Days.Wednesday), "eng"),
                    formLanguage = "English",
                };
            }

            List<string> ToLer(IEnumerable<LearningTime> learningTimes, string engOrHeb = "heb")
            {
                return (from l in learningTimes
                       from t in l.TimeInDay
                       select t.GetDescriptionFromEnumValue(engOrHeb))
                       .ToList();
            }
        }

        
            
    
}
