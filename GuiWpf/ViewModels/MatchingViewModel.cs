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

namespace GuiWpf.ViewModels
{
    public class MatchingViewModel : ViewModelBase
    {
        readonly IMatchingService _matchingService;

        readonly IEventAggregator _ea;

        public MatchingViewModel(IMatchingService matchingService, IEventAggregator ea, MatchCommand matchCommand)
        {
            _matchingService = matchingService;
            _ea = ea;
            Match = matchCommand;
            
            _ea.GetEvent<NewMatchEvent>()
                .Subscribe(async ps =>
                {
                    await Refresh();
                });
        }

        List<PairSuggestion> _pairSuggestions = new();

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
            IsLoaded = true;

            var suggestions = await _matchingService.GetAllPairSuggestions();

            _pairSuggestions.Clear();
            _pairSuggestions.AddRange(suggestions);

            Participants.Clear();
            Participants.AddRange(GroupPartBySuggestion(suggestions));            
            
            IsLoaded = false;
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

        private bool _isFullComparisonOpen;
        public bool IsFullComparisonOpen
        {
            get => _isFullComparisonOpen;
            set => SetProperty(ref _isFullComparisonOpen, value);
        }
        
        public ObservableCollection<ParticipaintWithSuggestions> Participants { get; set; } = new();

        private ParticipaintWithSuggestions _selectedParticipaint;
        public ParticipaintWithSuggestions SelectedParticipaint
        {
            get => _selectedParticipaint;
            set => SetProperty(ref _selectedParticipaint, value);
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
            IsFullComparisonOpen = true;
        });


        DelegateCommand _CloseFullComparisonCommand;
        public DelegateCommand CloseFullComparisonCommand => _CloseFullComparisonCommand ??= new(
        () =>
        {
            IsFullComparisonOpen = false;
        });

        public MatchCommand Match { get; }
    }
    
    public class MatchCommand : ICommand
    {
        IEventAggregator _ea;

        IPairsService _pairsService;
        
        IMatchingService _matchingService;

        public MatchCommand(IEventAggregator ea, IPairsService pairsService, IMatchingService matchingService)
        {
            _ea = ea;
            _pairsService = pairsService;
            _matchingService = matchingService;
        }
        async Task Match(PairSuggestion pairSuggestion)
        {
            var newPair = await _pairsService.AddNewPair(pairSuggestion);

            await _matchingService.Refresh();

            _ea.GetEvent<NewMatchEvent>()
                    .Publish(pairSuggestion);
            _ea.GetEvent<NewPairEvent>()
                .Publish(newPair);

        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }
        
        public async void Execute(object? parameter)
        {
            if(parameter is PairSuggestion pair)
            {
                await Match(pair);            
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
