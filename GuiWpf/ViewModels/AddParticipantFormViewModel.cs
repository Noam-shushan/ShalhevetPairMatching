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

        private List<string> _otherLanguages;
        public List<string> OtherLanguages
        {
            get => _otherLanguages;
            set => SetProperty(ref _otherLanguages, value);
        }

        DelegateCommand _addCommand;
        public DelegateCommand AddCommand => _addCommand ??= new(
        () =>
        {
            _ea.GetEvent<AddParticipantEvent>().Publish(new Participant
            {
                Email = Email,
                Gender = Gender,
                Name = Name,
                PhoneNumber = PhoneNumber,
                Country = Country.Country,
                PairPreferences = new()
                {
                    Tracks = new []{ PrefferdTrack }
                },
                DateOfRegistered = DateTime.Now,
                IsDeleted = false,
                IsInArchive = false 

            });
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
