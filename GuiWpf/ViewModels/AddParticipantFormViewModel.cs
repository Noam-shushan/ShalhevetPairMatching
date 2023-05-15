using Prism.Commands;
using Prism.Mvvm;
using PairMatching.DomainModel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Tools;
using Prism.Ioc;
using Prism.Events;
using PairMatching.Models;
using GuiWpf.Events;
using System.Collections.ObjectModel;
using static PairMatching.Tools.HelperFunction;


namespace GuiWpf.ViewModels
{
    public class AddParticipantFormViewModel : ViewModelBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly ExceptionHeandler _exceptionHeandler;

        public AddParticipantFormViewModel(IParticipantService participantService, IEventAggregator ea, ExceptionHeandler exceptionHeandler)
        {
            _participantService = participantService;
            _ea = ea;
            _exceptionHeandler = exceptionHeandler;

            CountryUtcs = _participantService.GetCountryUtcs();

            SubscaribeToEvents();
        }

        #region Properties
        private Participant _editParticipaint = new();
        public Participant EditParticipaint
        {
            get => _editParticipaint;
            set => SetProperty(ref _editParticipaint, value);
        }

        private string _title = "משתתף חדש";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }


        private Dictionary<Tuple<Days, TimesInDay>, bool> _openTimes;
        public Dictionary<Tuple<Days, TimesInDay>, bool> OpenTimes
        {
            get => _openTimes;
            set => SetProperty(ref _openTimes, value);
        }

        public IEnumerable<CountryUtc> CountryUtcs { get; init; }

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

        DelegateCommand _addCommand;
        public DelegateCommand AddCommand => _addCommand ??= new(
        async () =>
        {
            Participant newParticipant;
            var preferences = new Preferences
            {
                Tracks = new[] { PrefferdTrack },
                Gender = PrefferdGender,
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
                                   TimeInDay = day.SelectMany(t => t.TimeInDay).ToList()
                               },
                LearningStyle = LearningStyle,
                NumberOfMatchs = NumberOfMatchs
            };
            if (IsFromIsrael)
            {
                newParticipant = new IsraelParticipant
                {
                    Country = "Israel",
                    DateOfRegistered = DateTime.Now,
                    Email = Email,
                    Gender = Gender,
                    IsDeleted = false,
                    IsInArchive = false,
                    Name = Name,
                    PhoneNumber = PhoneNumber,
                    PairPreferences = preferences,
                    DesiredSkillLevel = DesiredSkillLevel,
                    EnglishLevel = EnglishLevel,
                    OpenQuestions = new OpenQuestionsForIsrael
                    {
                        InfoOnAdd = InfoOnAdd
                    }
                };
            }
            else
            {
                newParticipant = new WorldParticipant
                {
                    DateOfRegistered = DateTime.Now,
                    Email = Email,
                    Country = Country.Country,
                    UtcOffset = Country.UtcOffset,
                    SkillLevel = SkillLevel,
                    Gender = Gender,
                    IsDeleted = false,
                    IsInArchive = false,
                    Name = Name,
                    PhoneNumber = PhoneNumber,
                    PairPreferences = preferences,
                    DesiredEnglishLevel = DesiredEnglishLevel,
                    OpenQuestions = new OpenQuestionsForWorld
                    {
                        InfoOnAdd = InfoOnAdd
                    }
                };
            }
            try
            {
                IsLoaded = true;
                var newPart = await _participantService.InsertParticipant(newParticipant);
                _ea.GetEvent<AddParticipantEvent>().Publish(newPart);
                _ea.GetEvent<RefreshMatchingEvent>().Publish();
                IsLoaded = false;
                Reset();
                _ea.GetEvent<CloseDialogEvent>().Publish(false);
            }
            catch (Exception e)
            {
                IsLoaded = false;
                _exceptionHeandler.HeandleException(e);
            }
        }, () => !IsLoaded);


        DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand => _CancelCommand ??= new(
        () =>
        {
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });


        DelegateCommand _EditCommand;
        public DelegateCommand EditCommand => _EditCommand ??= new(
        async () =>
        {
            if (!Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך לבצע שינויים אלו?"))
            {
                return;
            }
            IsLoaded = true;
            EditParticipaint.PhoneNumber = PhoneNumber;
            EditParticipaint.Email = Email;
            EditParticipaint.Name = Name;

            EditParticipaint.PairPreferences.Tracks =
                Tracks
                .Where(tv => tv.Value)
                .Select(t => t.Key);
            
            EditParticipaint.PairPreferences.NumberOfMatchs = NumberOfMatchs;
            EditParticipaint.PairPreferences.Gender = PrefferdGender;
            EditParticipaint.PairPreferences.LearningStyle = LearningStyle;
            EditParticipaint.PairPreferences.LearningTime =
                        from lt in from ot in OpenTimes
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
                            TimeInDay = day.SelectMany(t => t.TimeInDay).ToList()
                        };
            bool isOpenToMathc = false;
            bool isChangeCountry = false;
            if (IsFromIsrael)
            {
                var israelPart = EditParticipaint.CopyPropertiesToNew<Participant ,IsraelParticipant>();
                israelPart.Country = "Israel";
                if (EditParticipaint is IsraelParticipant ip)
                {
                    israelPart.OpenQuestions = ip.OpenQuestions;
                }
                else if (EditParticipaint is WorldParticipant wp)
                {
                    ConvertWorldToIsrael(israelPart, wp);
                    isChangeCountry = true;
                }
                israelPart.EnglishLevel = EnglishLevel;
                israelPart.DesiredSkillLevel = DesiredSkillLevel;
                try
                {
                    await _participantService.UpdateParticipaint(israelPart, isChangeCountry);
                    _ea.GetEvent<ParticipaintWesUpdate>().Publish(israelPart);
                    isOpenToMathc = israelPart.IsOpenToMatch;
                }
                catch (Exception e)
                {
                    _exceptionHeandler.HeandleException(e);
                }
            }
            else
            {
                var worldPart = EditParticipaint.CopyPropertiesToNew<Participant, WorldParticipant>();

                if (EditParticipaint is IsraelParticipant ip)
                {
                    ConvertIsraelToWorld(ip, worldPart);
                    isChangeCountry = true;
                }
                else if (EditParticipaint is WorldParticipant wp)
                {
                    worldPart.OpenQuestions = wp.OpenQuestions;
                }

                worldPart.SkillLevel = SkillLevel;
                worldPart.DesiredEnglishLevel = DesiredEnglishLevel;
                worldPart.Country = Country.Country;
                worldPart.UtcOffset = Country.UtcOffset;
                try
                {
                    await _participantService.UpdateParticipaint(worldPart, isChangeCountry);
                    _ea.GetEvent<ParticipaintWesUpdate>().Publish(worldPart);
                    isOpenToMathc = worldPart.IsOpenToMatch;
                }
                catch (Exception e)
                {
                    _exceptionHeandler.HeandleException(e);
                }
            }
            IsLoaded = false;
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
            if (isOpenToMathc)
            {
                _ea.GetEvent<RefreshMatchingEvent>().Publish();
            }
        }, () => !IsLoaded);

        #endregion

        private void ConvertIsraelToWorld(IsraelParticipant israelParticipant, WorldParticipant worldParticipant)
        {
            worldParticipant.OpenQuestions = new OpenQuestionsForWorld
            {
                AdditionalInfo = israelParticipant.OpenQuestions.AdditionalInfo,
                WhoIntroduced = israelParticipant.OpenQuestions.WhoIntroduced,
                GeneralInfo =
                $"פרטים ביוגרפיים:\n {israelParticipant.OpenQuestions.BiographHeb}\n\n" +
                $"תכונות אישיות:\n {israelParticipant.OpenQuestions.PersonalTraits}\n\n" +
                $"למה הצטרפת לשלהבת: \n {israelParticipant.OpenQuestions.WhyJoinShalhevet}\n"
            };
            worldParticipant.Address = new();
        }

        private void ConvertWorldToIsrael(IsraelParticipant israelParticipant, WorldParticipant worldParticipant)
        {
            israelParticipant.OpenQuestions = new OpenQuestionsForIsrael
            {
                AdditionalInfo = worldParticipant.OpenQuestions.AdditionalInfo,
                WhoIntroduced = worldParticipant.OpenQuestions.WhoIntroduced,
                GeneralInfo = $"Personal Background:\n{worldParticipant.OpenQuestions.PersonalBackground}\n\n" +
                $"Experience:\n {worldParticipant.OpenQuestions.Experience}\n\n" +
                $"Requests From Pair:\n{worldParticipant.OpenQuestions.RequestsFromPair}\n\n" +
                $"JewishAndComAff:\n{worldParticipant.OpenQuestions.JewishAndComAff}\n\n" +
                $"Conversion Rabi:\n{worldParticipant.OpenQuestions.ConversionRabi}\n\n" +
                $"Anything Else:\n{worldParticipant.OpenQuestions.AnythingElse}\n\n" +
                $"Expectations:\n{string.Join("\t\n", worldParticipant.OpenQuestions.HopesExpectations.Select(s => $"• {s}"))}\n\n"
            };
        }

        private void Reset()
        {
            IsEdit = false;
            EditParticipaint = new();
            Name = Email = PhoneNumber = string.Empty;
            Title = "משתתף חדש";
            Country = new();
            Gender = default;
            SkillLevel = default;
            PrefferdTrack = default;
            OpenTimes = new();
            Tracks = new();
        }
        
        private void HandleNewEdit(Participant part)
        {
            IsEdit = true;
            Title = $"ערוך את {part.Name}";
            EditParticipaint = part;
            Name = part.Name;
            PhoneNumber = part.PhoneNumber;
            Email = part.Email;
            Gender = part.Gender;
            var selectedCountry = CountryUtcs.FirstOrDefault(c => CompereOnlyLetters(c.Country, part.Country));
            Country = selectedCountry ?? CountryUtcs.First();
            IsFromIsrael = part.IsFromIsrael;

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

        private void SubscaribeToEvents()
        {
            _ea.GetEvent<NewParticipaintEvent>()
                .Subscribe(isNew =>
                {
                    if (isNew)
                    {
                        Reset();
                    }
                });

            _ea.GetEvent<EditParticipaintEvent>()
                .Subscribe(part =>
                {
                    HandleNewEdit(part);
                });
        }

    }
}
