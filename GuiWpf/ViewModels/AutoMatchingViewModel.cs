using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using PairMatching.DomainModel.Services;
using PairMatching.DomainModel.MatchingCalculations;
using System.Collections.ObjectModel;
using GuiWpf.Events;
using Prism.Events;

namespace GuiWpf.ViewModels
{
    public class AutoMatchingViewModel : ViewModelBase
    {
        readonly IMatchingService _matchingService;

        readonly IEventAggregator _ea;        

        public AutoMatchingViewModel(IMatchingService matchingService, IEventAggregator ea)
        {
            _matchingService = matchingService;
            _ea = ea;
            
            _ea.GetEvent<RefreshMatchingEvent>()
                .Subscribe(async () =>
                {
                    IsFullComparisonOpen = false;
                    await Refresh();
                });
        }
        
        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
        async () =>
        {
            await Refresh();
            IsInitialized = true;
        },
        () => !IsInitialized && !IsLoaded);

        private async Task Refresh()
        {
            IsLoaded = true;
            
            var result = await _matchingService.GetMaxOptMatching();
            AutoSuggestions.Clear();
            AutoSuggestions.AddRange(result);
            
            IsLoaded = false;
        }

        public ObservableCollection<PairSuggestion> AutoSuggestions { get; } = new();


        private PairSuggestion _selectedSuggestions;
        public PairSuggestion SelectedSuggestions
        {
            get => _selectedSuggestions;
            set => SetProperty(ref _selectedSuggestions, value);
        }

        DelegateCommand _OpenFullComparisonCommand;
        public DelegateCommand OpenFullComparisonCommand => _OpenFullComparisonCommand ??= new(
        () =>
        {
            _ea.GetEvent<ShowFullComparisonEvent>().Publish((AutoSuggestions
                //.Where(ps => 
                //ps.FromIsrael.Id == SelectedSuggestions.FromIsrael.Id
                //&& ps.FromWorld.Id == SelectedSuggestions.FromWorld.Id)
                , SelectedSuggestions.FromIsrael.Id));
            IsFullComparisonOpen = true;
        });

        private bool _isFullComparisonOpen;
        public bool IsFullComparisonOpen
        {
            get => _isFullComparisonOpen;
            set => SetProperty(ref _isFullComparisonOpen, value);
        }

        DelegateCommand _CloseFullComparisonCommand;
        public DelegateCommand CloseFullComparisonCommand => _CloseFullComparisonCommand ??= new(
        () =>
        {
            IsFullComparisonOpen = false;
        });
    }
}
