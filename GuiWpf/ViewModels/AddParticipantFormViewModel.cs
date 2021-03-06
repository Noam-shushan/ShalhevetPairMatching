using Prism.Commands;
using Prism.Mvvm;
using PairMatching.DomainModel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Events;
using PairMatching.Models;
using GuiWpf.Events;

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

        private CountryUtc _country;
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

        private List<LearningTime> _learningTimes;
        public List<LearningTime> LearningTimes
        {
            get => _learningTimes;
            set => SetProperty(ref _learningTimes, value);
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

        DelegateCommand _addCommand;
        public DelegateCommand AddCommand => _addCommand ??= new(
        () =>
        {
            Participant newPart;
            var pref = new Preferences
            {
                Tracks = new[] { PrefferdTrack },
                Gender = PrefferdGender,
                LearningTime = LearningTimes,
                LearningStyle = LearningStyle
            };
            if(IsFromIsrael)
            {
                newPart = new IsraelParticipant
                {
                    Country = "Israel",
                    DateOfRegistered = DateTime.Now,
                    Email = Email,
                    Gender = Gender,
                    IsDeleted = false,
                    IsInArchive = false,
                    Name = Name,
                    PhoneNumber = PhoneNumber,
                    PairPreferences = pref,
                    DesiredSkillLevel = DesiredSkillLevel,
                    EnglishLevel = EnglishLevel,  
                };
            }
            else
            {
                newPart = new WorldParticipant
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
                    PairPreferences = pref,
                    DesiredEnglishLevel = DesiredEnglishLevel
                };
            }
            _ea.GetEvent<AddParticipantEvent>().Publish(newPart);
            Reset();
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });


        DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand => _CancelCommand ??= new(
        () =>
        { // ask natan if to clear her the props   
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
        });

        private void Reset()
        {
            Name = Email = PhoneNumber = string.Empty;
            Country = new();
            Gender = default;
            SkillLevel = default;
            PrefferdTrack = default;
        }

    }
}
