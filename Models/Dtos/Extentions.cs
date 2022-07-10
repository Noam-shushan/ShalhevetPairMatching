using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public static class DtoExtentions
    {
        public static UpsertParticipantOnWixDto ToUpsertWixDto(this Participant part)
        {
            var partDto = new UpsertParticipantOnWixDto
            {
                _id = part.WixId,
                email = part.Email,
                gender = part.Gender.GetDescriptionFromEnumValue(),
                fullName = part.Name,

                moreThanOneChevruta = part.PairPreferences.NumberOfMatchs.ToString(),
                tel = part.PhoneNumber,
                menOrWomen = part.PairPreferences.Gender.GetDescriptionFromEnumValue(),
                monday = (from lt in part.PairPreferences.LearningTime
                          where lt.Day == Days.Monday
                          select lt.TimeInDay)
                                         .FirstOrDefault()
                                         .Select(t => t.GetDescriptionFromEnumValue())
                                         .ToList(),
                sunday = (from lt in part.PairPreferences.LearningTime
                          where lt.Day == Days.Sunday
                          select lt.TimeInDay)
                                          .FirstOrDefault()
                                          .Select(t => t.GetDescriptionFromEnumValue())
                                          .ToList(),
                tuseday = (from lt in part.PairPreferences.LearningTime
                           where lt.Day == Days.Tuesday
                           select lt.TimeInDay)
                                           .FirstOrDefault()
                                           .Select(t => t.GetDescriptionFromEnumValue())
                                           .ToList(),
                wednesday = (from lt in part.PairPreferences.LearningTime
                             where lt.Day == Days.Wednesday
                             select lt.TimeInDay)
                                             .FirstOrDefault()
                                             .Select(t => t.GetDescriptionFromEnumValue())
                                             .ToList(),
                thurseday = (from lt in part.PairPreferences.LearningTime
                             where lt.Day == Days.Thursday
                             select lt.TimeInDay)
                                             .FirstOrDefault()
                                             .Select(t => t.GetDescriptionFromEnumValue())
                                             .ToList(),
                otherLan = part.OtherLanguages,
                preferredTrack = part.PairPreferences
                                .Tracks
                                .Select(t => t.GetDescriptionFromEnumValue())
                                .ToList(),
                country = part.Country,

                learningStyle = part.PairPreferences.LearningStyle.GetDescriptionFromEnumValue()
            };
            if (part.IsFromIsrael)
            {
                var ip = part as IsraelParticipant;
                partDto.levOfEn = ip.EnglishLevel.GetDescriptionFromEnumValue();
                partDto.chevrotaSkills = ip.DesiredSkillLevel.GetDescriptionFromEnumValue();
            }
            else
            {
                var wp = part as WorldParticipant;
                partDto.timeOffset = wp.UtcOffset.Hours;
                partDto.learningSkill = wp.SkillLevel.GetDescriptionFromEnumValue();
                partDto.chevrotalevOfEn = wp.DesiredEnglishLevel.GetDescriptionFromEnumValue();
            }

            return partDto;
        }
    }
}
