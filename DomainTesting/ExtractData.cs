using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DataAccess.UnitOfWork;
using PairMatching.Configurations;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Tools;

namespace DomainTesting
{
    [TestFixture]
    public class ExtractData
    {
        readonly IUnitOfWork _unitOfWork;
        public ExtractData()
        {
            var conf = GetConfigurations();
            var db = new DataAccessFactory(conf);
            _unitOfWork = db.GetDataAccess();
        }

        private MyConfiguration GetConfigurations()
        {
            var jsonString = ReadJson(@"C:\Users\Asuspcc\source\Repos\ShalhevetPairMatching\GuiWpf\Resources\appsetting.json");
            var configurations = JsonConvert.DeserializeObject<MyConfiguration>(jsonString);
            return configurations ?? throw new Exception("No Configurations");
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
            await _unitOfWork.StudentRepositry.SaveToDrive(l, "Shalhevet data");
        }

    }
}
