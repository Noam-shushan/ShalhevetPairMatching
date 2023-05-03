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

namespace GuiWpf.ViewModels
{
    public class AddParticipantFormViewModel : BindableBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;

        public AddParticipantFormViewModel(IParticipantService participantService, IEventAggregator ea)
        {
            _participantService = participantService;
            _ea = ea;
            CountryUtcs = _participantService.GetCountryUtcs();

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
                    IsEdit = true;
                    Title = $"ערוך את {part.Name}";
                    EditParticipaint = part;
                    Name = part.Name;
                    PhoneNumber = part.PhoneNumber;
                    Email = part.Email;
                    Gender = part.Gender;
                    Country.Country = part.Country;
                    IsFromIsrael = part.IsFromIsrael;

                    PrefferdTrack = part.PairPreferences.Tracks.FirstOrDefault();
                    PrefferdGender = part.PairPreferences.Gender;
                    NumberOfMatchs = part.PairPreferences.NumberOfMatchs;
                    OtherLanguages = part.OtherLanguages;
                    LearningStyle = part.PairPreferences.LearningStyle;
                    
                    LearningTimes.Clear();
                    LearningTimes.AddRange(part.PairPreferences.LearningTime);
                    
                    if (part.IsFromIsrael && part is IsraelParticipant)
                    {
                        var ipart = part as IsraelParticipant;
                        EnglishLevel = ipart.EnglishLevel;
                        DesiredSkillLevel = ipart.DesiredSkillLevel;
                    }
                    else if(!part.IsFromIsrael && part is WorldParticipant)
                    {
                        var wpart = part as WorldParticipant;
                        SkillLevel = wpart.SkillLevel;
                        DesiredEnglishLevel = wpart.DesiredEnglishLevel;
                        Country.UtcOffset = wpart.UtcOffset;
                    }
                    else
                    {

                    }

                });
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


        private ObservableCollection<LearningTime> _learningTimes = new();
        public ObservableCollection<LearningTime> LearningTimes
        {
            get => _learningTimes;
            set => SetProperty(ref _learningTimes, value);
        }
        #endregion

        DelegateCommand<object> _SelectTimeInDayCommand;
        public DelegateCommand<object> SelectTimeInDayCommand => _SelectTimeInDayCommand ??= new(
        (timeInDayParam) =>
        {
            if (timeInDayParam is Tuple<Days, TimesInDay> timeInDay)
            {
                LearningTimes.Add(new LearningTime
                {
                    Day = timeInDay.Item1,
                    TimeInDay = new List<TimesInDay> { timeInDay.Item2 }
                });
            }
        });

        DelegateCommand<object> _RemoveTimeInDayCommand;
        public DelegateCommand<object> RemoveTimeInDayCommand => _RemoveTimeInDayCommand ??= new(
        (timeInDayParam) =>
        {
            if (timeInDayParam is Tuple<Days, TimesInDay> timeInDay)
            {
                LearningTimes.Remove(new LearningTime
                {
                    Day = timeInDay.Item1,
                    TimeInDay = new List<TimesInDay> { timeInDay.Item2 }
                });
            }
        });

        DelegateCommand _addCommand;
        public DelegateCommand AddCommand => _addCommand ??= new(
        () =>
        {
            Participant newParticipant;
            var preferences = new Preferences
            {
                Tracks = new[] { PrefferdTrack },
                Gender = PrefferdGender,
                LearningTime = from lt in LearningTimes
                               group lt by lt.Day into day
                               select new LearningTime
                               {
                                   Day = day.Key,
                                   TimeInDay = day.SelectMany(t => t.TimeInDay).ToList()
                               },
                LearningStyle = LearningStyle,
                NumberOfMatchs = NumberOfMatchs
            };
            if(IsFromIsrael)
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
                };
            }
            _ea.GetEvent<AddParticipantEvent>().Publish(newParticipant);
            _ea.GetEvent<RefreshMatchingEvent>().Publish();
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });


        DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand => _CancelCommand ??= new(
        () =>
        { // ask natan if to clear her the props
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });


        DelegateCommand _EditCommand;
        public DelegateCommand EditCommand => _EditCommand ??= new(
        async () =>
        {
            if(!Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך לבצע שינויים אלו?"))
            {
                Reset();
                return;
            }
            EditParticipaint.PhoneNumber = PhoneNumber;
            EditParticipaint.Email = Email;
            EditParticipaint.Country = Country.Country;
            EditParticipaint.Name = Name;
            EditParticipaint.PairPreferences.Tracks = new PrefferdTracks[] { PrefferdTrack };
            EditParticipaint.PairPreferences.NumberOfMatchs = NumberOfMatchs;
            EditParticipaint.PairPreferences.Gender = PrefferdGender;
            EditParticipaint.PairPreferences.LearningStyle = LearningStyle;
            EditParticipaint.PairPreferences.LearningTime = LearningTimes;
            if(IsFromIsrael && EditParticipaint is IsraelParticipant)
            {
                (EditParticipaint as IsraelParticipant).EnglishLevel = EnglishLevel;
                (EditParticipaint as IsraelParticipant).DesiredSkillLevel = DesiredSkillLevel;
            }
            else if (!IsFromIsrael && EditParticipaint is WorldParticipant)
            {
                (EditParticipaint as WorldParticipant).SkillLevel = SkillLevel;
                (EditParticipaint as WorldParticipant).DesiredEnglishLevel = DesiredEnglishLevel;
                (EditParticipaint as WorldParticipant).UtcOffset = Country.UtcOffset;
            }
            else
            {

            }
            await _participantService.UpdateParticipaint(EditParticipaint);
            _ea.GetEvent<ParticipaintWesUpdate>().Publish(EditParticipaint);
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
            _ea.GetEvent<RefreshMatchingEvent>().Publish();
        });

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
        }

    }
}
