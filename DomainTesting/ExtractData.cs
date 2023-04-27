using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DataAccess.UnitOfWorks;
using PairMatching.Configurations;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Tools;
using PairMatching.WixApi;
using PairMatching.ExcelTool;
using PairMatching.GoogleSheet;
using PairMatching.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using PairMatching.Root;

namespace DomainTesting
{
    public class StudentComparer : IEqualityComparer<Student>
    {
        public bool Equals(Student? x, Student? y)
        {
            return x.Name == y.Name && x.Email == y.Email;
        }

        public int GetHashCode([DisallowNull] Student obj)
        {
            return obj.GetHashCode();
        }
    }
    
    [TestFixture]
    public class ExtractData
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        readonly MyConfiguration _config;
        public ExtractData()
        {
            _config = new Startup()
                .GetConfigurations();
            var db = new DataAccessFactory(_config);
            _unitOfWork = db.GetDataAccess();
            _wix = new WixDataReader(_config);
        }
        
        [Test]
        public async Task ExtractEmail()
        {
            var students = await _unitOfWork.StudentRepositry.GetAllAsync(s=> !s.IsDeleted);
            var paris = await _unitOfWork.OldPairsRepositry.GetAllAsync(p => !p.IsDeleted);

            var l =
                    from s in students
                    let charota = students.FirstOrDefault(s1 => s.MatchTo.Contains(s1.Id))
                    let charotaTrack = paris.FirstOrDefault(
                        op =>
                        (op.StudentFromIsraelId == s.Id && s.IsFromIsrael && charota?.Id == op.StudentFromWorldId)
                        || (op.StudentFromWorldId == s.Id && !s.IsFromIsrael && charota?.Id == op.StudentFromIsraelId))
                    ?.PrefferdTracks                 
                    select new
                    {
                        Name = s.Name,
                        Phone = s.PhoneNumber,
                        Email = s.Email,
                        Chavrota = new { Name = charota?.Name, Email = charota?.Email, Track = charotaTrack?.GetDescriptionFromEnumValue() },
                        Country = s.Country,
                        LearningStyle = s.LearningStyle.GetDescriptionFromEnumValue(),                       
                        Track = string.Join(", ", s.PrefferdTracks.Select(t => t.GetDescriptionFromEnumValue())),
                        Gender = s.Gender.GetDescriptionFromEnumValue(),
                        PreliminaryLevelOfKnowledge = s.SkillLevel.GetDescriptionFromEnumValue(),
                        OpenQuestions = s.OpenQuestions?.ToDictionary(q => q.Question, a => a.Answer),
                        
                    };
        }

        [Test]
        public async Task FixDBBugs()
        {
            var students = await _unitOfWork
                .StudentRepositry
                .GetAllAsync(s => !s.IsSimpleStudent && !s.IsDeleted);

            var ips = await _unitOfWork
                .IsraelParticipantsRepositry.GetAllAsync(p => !p.IsDeleted);
            var wps = await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync(p => !p.IsDeleted);

            Assert.AreEqual(students.Count(), ips.Count() + wps.Count());
        }

        [Test]
        public async Task CreateExcelTest()
        {
            var eg = new ExcelGenerator("stam", "stam");

            var parts = await _wix.GetNewParticipants();

            var fromDate = DateTime.Parse("1/6/2022");
            
            var studentsFromGS = await GetStudentsFromGoogleSheetTest();

            var last2ManthStudentsFromGS = studentsFromGS.Where(s => s.DateOfRegistered > fromDate);
            
            var orderGS = last2ManthStudentsFromGS.OrderByDescending(s => s.DateOfRegistered);
            var values =
                (from p in parts
                 select new
                 {
                     Name = p.fullName,
                     Email = p.email,
                     Phone = p.tel
                 }).Union
                (from s in orderGS
                 select new
                 {
                     s.Name,
                     s.Email,
                     Phone = s.PhoneNumber
                 });


            await eg.Generate(values, "כולם ביחד, Wix & Google Forms (מהחודשיים הראשונים)");
        }
        
        
        public async Task<List<Student>> GetStudentsFromGoogleSheetTest()
        {
            var tempRang = "A2:Z";
            var range = new SpredsheetLastRange()
            {
                EnglishSheets = tempRang,
                HebrewSheets = tempRang
            }; // await _unitOfWork
            //                .ConfigRepositry
            //                .GetSpredsheetLastRange();

            var gs = new GoogleSheetParser();
            await gs.ReadAsync(new HebrewDescriptor(range, _config));
            await gs.ReadAsync(new EnglishDiscriptor(range, _config));
           
            var studentsFromGS = gs.NewStudents;
            return studentsFromGS;
        }
        
        [Test]
        public async Task GetFromGoogleSheetTest()
        {
            var students = await GetStudentsFromGoogleSheetTest();
            
            var ips = await _unitOfWork.IsraelParticipantsRepositry
                .GetAllAsync();
            var ipsEmails = ips.Select(i => i.Email).ToList();
            
            var wps = await _unitOfWork.WorldParticipantsRepositry
                .GetAllAsync();
            var wpsEmails = wps.Select(i => i.Email).ToList();

            var parts = students.Select(s => s.ToParticipant());

            var newFromGoogleSheets = from p in parts
                                      where !ipsEmails.Contains(p.Email) && !wpsEmails.Contains(p.Email)
                                      select p;

            var partsIW = newFromGoogleSheets.ToLookup(p => p.IsFromIsrael);

            var newIsraelis = partsIW[true].Select(p => p as IsraelParticipant);

            await _unitOfWork.IsraelParticipantsRepositry.InsertMany(newIsraelis);

            var newWorlds = partsIW[false].Select(p => p as WorldParticipant);

            await _unitOfWork.WorldParticipantsRepositry.InsertMany(newWorlds);
        }
    }
}
