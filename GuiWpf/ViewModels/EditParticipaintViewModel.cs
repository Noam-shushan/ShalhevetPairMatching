using PairMatching.DomainModel.Services;
using PairMatching.Models;
using PairMatching.Tools;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiWpf.Events;

namespace GuiWpf.ViewModels
{
    public class EditParticipaintViewModel : ViewModelBase, IPopupViewModle
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly ExceptionHeandler _exceptionHeandler;
        
        public EditParticipaintViewModel(IParticipantService participantService, IEventAggregator ea, ExceptionHeandler exceptionHeandler)
        {
            _participantService = participantService;
            _ea = ea;
            _exceptionHeandler = exceptionHeandler;

            EditParticipaint.CountryUtcs = _participantService.GetCountryUtcs();

            ExitPopupVM = new ExitPopupModelViewModel(this);
        }

        public ExitPopupModelViewModel ExitPopupVM { get; set; }

        private ParticipaintData _participaint = new();
        public ParticipaintData EditParticipaint
        {
            get => _participaint;
            set => SetProperty(ref _participaint, value);
        }

        private Participant _original = new();

        public void Init(Participant participaint, bool isOpen)
        {
            _original = participaint;
            EditParticipaint.InitEdit(participaint);
            Title = $"ערוך את {participaint.Name}";
            IsOpen = isOpen;
        }

        private string _title = "";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        DelegateCommand _EditCommand;
        public DelegateCommand EditCommand => _EditCommand ??= new(
        async () =>
        {
            if (!Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך לבצע שינויים אלו?"))
            {
                return;
            }
            IsLoaded = true;
            _original.PhoneNumber = EditParticipaint.PhoneNumber;
            _original.Email = EditParticipaint.Email;
            _original.Name = EditParticipaint.Name;
            _original.PairPreferences = EditParticipaint.GetPreference();

            bool isOpenToMathc = false;
            bool isChangeCountry = false;
            if (EditParticipaint.IsFromIsrael)
            {
                var israelPart = _original.CopyPropertiesToNew<Participant, IsraelParticipant>();
                israelPart.Country = "Israel";
                if (_original is IsraelParticipant ip)
                {
                    israelPart.OpenQuestions = ip.OpenQuestions;
                }
                else if (_original is WorldParticipant wp)
                {
                    ConvertWorldToIsrael(israelPart, wp);
                    isChangeCountry = true;
                }
                israelPart.EnglishLevel = EditParticipaint.EnglishLevel;
                israelPart.DesiredSkillLevel = EditParticipaint.DesiredSkillLevel;
                try
                {
                    await _participantService.UpdateParticipaint(israelPart, isChangeCountry);
                    //what happend before is that when updating a participant from world to isreal it made a new israel 
                    //and deleted the old israel participant
                    //so i made it so it will delete the old participant form the world participants

                    //_onSave(israelPart);
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
                var worldPart = _original.CopyPropertiesToNew<Participant, WorldParticipant>();

                if (_original is IsraelParticipant ip)
                {
                    ConvertIsraelToWorld(ip, worldPart);
                    isChangeCountry = true;
                }
                else if (_original is WorldParticipant wp)
                {
                    worldPart.OpenQuestions = wp.OpenQuestions;
                }

                worldPart.SkillLevel = EditParticipaint.SkillLevel;
                worldPart.DesiredEnglishLevel = EditParticipaint.DesiredEnglishLevel;
                worldPart.Country = EditParticipaint.Country.Country;
                worldPart.UtcOffset = EditParticipaint.Country.UtcOffset;
                try
                {
                    await _participantService.UpdateParticipaint(worldPart, isChangeCountry);
                    //_onSave(worldPart);
                    _ea.GetEvent<ParticipaintWesUpdate>().Publish(worldPart);
                    isOpenToMathc = worldPart.IsOpenToMatch;
                }
                catch (Exception e)
                {
                    _exceptionHeandler.HeandleException(e);
                }
            }
            IsLoaded = false;
            
            if (isOpenToMathc)
            {
                _ea.GetEvent<RefreshMatchingEvent>().Publish();
            }
            CloseDialog();
        }, () => !IsLoaded);

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

        public void CloseDialog()
        {
            IsOpen = false;
        }
    }
}
