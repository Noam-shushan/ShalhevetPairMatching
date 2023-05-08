using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using Prism.Commands;
using Prism.Events;
using GuiWpf.Events;
using GuiWpf.UIModels;
using GuiWpf.Commands;

namespace GuiWpf.ViewModels
{
    public class MatchingViewModel : ViewModelBase
    {
        readonly IMatchingService _matchingService;

        readonly IEventAggregator _ea;

        readonly ExceptionHeandler _exceptionHeandler;

        readonly IParticipantService _participantService;

        public MatchingViewModel(IMatchingService matchingService, IParticipantService participantService, IEventAggregator ea, MatchCommand matchCommand, ExceptionHeandler exceptionHeandler)
        {
            _matchingService = matchingService;
            _ea = ea;
            Match = matchCommand;
            _exceptionHeandler = exceptionHeandler;
            _participantService = participantService;
            FullParticipaintVm = new();

            _ea.GetEvent<RefreshMatchingEvent>()
                .Subscribe(async () =>
                {
                    await Refresh();
                });
        }

        private bool _isFreeMatchSelectionOpen;
        public bool IsFreeMatchSelectionOpen
        {
            get => _isFreeMatchSelectionOpen;
            set => SetProperty(ref _isFreeMatchSelectionOpen, value);
        }

        public ObservableCollection<Participant> ParticipantsForFreeMatch
        {
            get;
        } = new();

        private Participant _selectedParticipaintForFreeMatch;
        public Participant SelectedParticipaintForFreeMatch
        {
            get => _selectedParticipaintForFreeMatch;
            set => SetProperty(ref _selectedParticipaintForFreeMatch, value);
        }

        List<PairSuggestion> _pairSuggestions = new();

        public FullParticipaintViewModel FullParticipaintVm { get; set; }


        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
               async () =>
               {
                   //await MetroProgressOnLoading();
                   await Refresh();
                   IsInitialized = true;
               },
               () => !IsInitialized || IsLoaded);

        private async Task Refresh()
        {
            try
            {
                IsLoaded = true;

                var suggestions = await _matchingService.GetAllPairSuggestions();

                _pairSuggestions.Clear();
                _pairSuggestions.AddRange(suggestions);

                var items = GroupPartBySuggestion(suggestions);
                Participants.Init(items, 20, ItemsFilter);


                IsLoaded = false;
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        }

        private bool ItemsFilter(ParticipaintWithSuggestions p)
        {
            return p.Participant.Name.Contains(SearchParticipiantsWord, StringComparison.InvariantCultureIgnoreCase);
        }

        private IEnumerable<ParticipaintWithSuggestions> GroupPartBySuggestion(IEnumerable<PairSuggestion> suggestions)
        {
            return (from p in suggestions
                    let ip = p.FromIsrael
                    group new { p.MatchingScore, p.FromWorld, p.MatchingPercent, Original = p } 
                    by new { ip.Id, ip } into ps
                    select new ParticipaintWithSuggestions
                    {
                        Participant = ps.Key.ip,
                        Suggestions = ps.Select(i => new ParticipantSuggestion
                        {
                            Country = i.FromWorld.Country,
                            Id = i.FromWorld.Id,
                            Name = i.FromWorld.Name,
                            MatchingPercent = i.MatchingPercent, 
                            Original = i.Original
                        }).ToList()
                    })
                    .Union(from p in suggestions
                            let wp = p.FromWorld
                            group new { p.MatchingScore, p.FromIsrael, p.MatchingPercent, Original = p } 
                            by new { wp.Id, wp } into ps
                            select new ParticipaintWithSuggestions
                            {
                                Participant = ps.Key.wp,
                                Suggestions = ps.Select(i => new ParticipantSuggestion
                                {
                                    Country = i.FromIsrael.Country,
                                    Id = i.FromIsrael.Id,
                                    Name = i.FromIsrael.Name,
                                    MatchingPercent = i.MatchingPercent,
                                    Original = i.Original                                    
                                }).ToList()
                            });
        }

        private string _searchParticipiantsWord = "";
        public string SearchParticipiantsWord
        {
            get { return _searchParticipiantsWord; }
            set
            {
                if (SetProperty(ref _searchParticipiantsWord, value))
                {
                    Participants.Refresh();
                }
            }
        }

        private PrefferdTracks _selectedTrack;
        public PrefferdTracks SelectedTrack
        {
            get => _selectedTrack;
            set => SetProperty(ref _selectedTrack, value);
        }

        public PaginCollectionViewModel<ParticipaintWithSuggestions> Participants { get; set; } = new();

        private ParticipaintWithSuggestions _selectedParticipaint = new();
        public ParticipaintWithSuggestions SelectedParticipaint
        {
            get => _selectedParticipaint;
            set
            {
                SetProperty(ref _selectedParticipaint, value);
            }
        }
        
        private ParticipantSuggestion _selectedSuggestion;
        public ParticipantSuggestion SelectedSuggestion
        {
            get => _selectedSuggestion;
            set => SetProperty(ref _selectedSuggestion, value);
        }

        DelegateCommand _OpenFullComparisonCommand;
        public DelegateCommand OpenFullComparisonCommand => _OpenFullComparisonCommand ??= new(
        () =>
        {
            var list = from ps in _pairSuggestions
                       where ps.FromIsrael.Id == SelectedParticipaint.Participant.Id
                       || ps.FromWorld.Id == SelectedParticipaint.Participant.Id
                       select ps;
            _ea.GetEvent<ShowFullComparisonEvent>()
            .Publish((list, SelectedSuggestion.Id));
            _ea.GetEvent<OpenCloseDialogEvent>().Publish((true, typeof(FullPairMatchingComparisonViewModel)));
        });


        DelegateCommand _OpenFullParticipiantCommand;
        public DelegateCommand OpenFullParticipiantCommand => _OpenFullParticipiantCommand ??= new(
        () =>
        {
            FullParticipaintVm.Init(SelectedParticipaint.Participant, true);
        });


        public MatchCommand Match { get; }
    }
    
}
