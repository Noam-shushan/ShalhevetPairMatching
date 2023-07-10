using PairMatching.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PairMatching.Tools.HelperFunction;
using PairMatching.Tools;


namespace GuiWpf.ViewModels
{
    public class ParticipaintData : BindableBase
    {   
        #region Properties
        private Dictionary<Tuple<Days, TimesInDay>, bool> _openTimes;
        public Dictionary<Tuple<Days, TimesInDay>, bool> OpenTimes
        {
            get => _openTimes;
            set => SetProperty(ref _openTimes, value);
        }

        public IEnumerable<CountryUtc> CountryUtcs { get; set; }
        
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private Genders _gender;
        public Genders Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private CountryUtc _country = new();
        public CountryUtc Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        private SkillLevels _skillLevel;
        public SkillLevels SkillLevel
        {
            get => _skillLevel;
            set => SetProperty(ref _skillLevel, value);
        }

        private PrefferdTracks _prefferdTrack;
        public PrefferdTracks PrefferdTrack
        {
            get => _prefferdTrack;
            set => SetProperty(ref _prefferdTrack, value);
        }

        private Dictionary<PrefferdTracks, bool> _traks = new();
        public Dictionary<PrefferdTracks, bool> Tracks
        {
            get => _traks;
            set => SetProperty(ref _traks, value);
        }

        private Genders _prefferdGender;
        public Genders PrefferdGender
        {
            get => _prefferdGender;
            set => SetProperty(ref _prefferdGender, value);
        }

        private LearningStyles _learningStyle;
        public LearningStyles LearningStyle
        {
            get => _learningStyle;
            set => SetProperty(ref _learningStyle, value);
        }

        private SkillLevels _desiredSkillLevel;
        public SkillLevels DesiredSkillLevel
        {
            get => _desiredSkillLevel;
            set => SetProperty(ref _desiredSkillLevel, value);
        }

        private EnglishLevels _desiredEnglishLevel;
        public EnglishLevels DesiredEnglishLevel
        {
            get => _desiredEnglishLevel;
            set => SetProperty(ref _desiredEnglishLevel, value);
        }

        private EnglishLevels _englishLevel;
        public EnglishLevels EnglishLevel
        {
            get => _englishLevel;
            set => SetProperty(ref _englishLevel, value);
        }

        private int _numberOfMatchs;
        public int NumberOfMatchs
        {
            get => _numberOfMatchs;
            set => SetProperty(ref _numberOfMatchs, value);
        }

        private List<string> _otherLanguages;
        public List<string> OtherLanguages
        {
            get => _otherLanguages;
            set => SetProperty(ref _otherLanguages, value);
        }

        private bool _isFromIsrael;
        public bool IsFromIsrael
        {
            get => _isFromIsrael;
            set => SetProperty(ref _isFromIsrael, value);
        }

        private string _infoOnAdd;
        public string InfoOnAdd
        {
            get => _infoOnAdd;
            set => SetProperty(ref _infoOnAdd, value);
        }

        private bool _isMatch;
        public bool IsMatch
        {
            get => _isMatch;
            set => SetProperty(ref _isMatch, value);
        }
        #endregion

        #region Commands
        DelegateCommand<object> _SelectTimeInDayCommand;
        public DelegateCommand<object> SelectTimeInDayCommand => _SelectTimeInDayCommand ??= new(
        (timeInDayParam) =>
        {
            if (timeInDayParam is Tuple<Days, TimesInDay> timeInDay)
            {
                OpenTimes[timeInDay] = true;
            }
        });

        DelegateCommand<object> _RemoveTimeInDayCommand;
        public DelegateCommand<object> RemoveTimeInDayCommand => _RemoveTimeInDayCommand ??= new(
        (timeInDayParam) =>
        {
            if (timeInDayParam is Tuple<Days, TimesInDay> timeInDay)
            {
                OpenTimes[timeInDay] = false;
            }
        });

        DelegateCommand<object> _SelectTrackCommand;
        public DelegateCommand<object> SelectTrackCommand => _SelectTrackCommand ??= new(
        (trackParam) =>
        {
            if (trackParam is string trackstr)
            {
                var track = Extensions.GetValueFromDescription<PrefferdTracks>(trackstr);
                Tracks[track] = true;
            }
        });

        DelegateCommand<object> _RemoveTrackCommand;
        public DelegateCommand<object> RemoveTrackCommand => _RemoveTrackCommand ??= new(
        (trackParam) =>
        {
            if (trackParam is string trackstr)
            {
                var track = Extensions.GetValueFromDescription<PrefferdTracks>(trackstr);
                Tracks[track] = false;
            }
        }); 
        #endregion

        public void InitEdit(Participant part)
        {
            Name = part.Name;
            PhoneNumber = part.PhoneNumber;
            Email = part.Email;
            Gender = part.Gender;
            var selectedCountry = CountryUtcs.FirstOrDefault(c => CompereOnlyLetters(c.Country, part.Country));
            Country = selectedCountry ?? CountryUtcs.First();
            IsFromIsrael = part.IsFromIsrael;
            IsMatch = part.IsMatch;

            PrefferdTrack = part.PairPreferences.Tracks.FirstOrDefault();
            Tracks = part.PairPreferences.Tracks
                .ToDictionary(t => t, _ => true);

            PrefferdGender = part.PairPreferences.Gender;
            NumberOfMatchs = part.PairPreferences.NumberOfMatchs;
            OtherLanguages = part.OtherLanguages;
            LearningStyle = part.PairPreferences.LearningStyle;

            OpenTimes = (from lt in part.PairPreferences.LearningTime
                         from time in lt.TimeInDay
                         select new Tuple<Days, TimesInDay>(lt.Day, time))
                        .ToDictionary(key => key, val => true);


            if (part.IsFromIsrael && part is IsraelParticipant ipart)
            {
                EnglishLevel = ipart.EnglishLevel;
                DesiredSkillLevel = ipart.DesiredSkillLevel;

            }
            else if (!part.IsFromIsrael && part is WorldParticipant wpart)
            {
                SkillLevel = wpart.SkillLevel;
                DesiredEnglishLevel = wpart.DesiredEnglishLevel;
            }

        }
        
        public Preferences GetPreference()
        {
            return new Preferences
            {
                Gender = PrefferdGender,
                LearningStyle = LearningStyle,
                NumberOfMatchs = NumberOfMatchs,
                Tracks = Tracks
                        .Where(tv => tv.Value)
                        .Select(t => t.Key).Distinct(),
                LearningTime = from lt in from ot in OpenTimes
                                          where ot.Value
                                          select new LearningTime
                                          {
                                              Day = ot.Key.Item1,
                                              TimeInDay = new List<TimesInDay> { ot.Key.Item2 }
                                          }
                               group lt by lt.Day into day
                               select new LearningTime
                               {
                                   Day = day.Key,
                                   TimeInDay = day.SelectMany(t => t.TimeInDay).Distinct()
                               }

            };
        }
    }
}
