using PairMatching.DomainModel.BLModels;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GuiWpf.Events;
using System.Threading.Tasks;

namespace GuiWpf.ViewModels
{
    public class StandbyMatchingViewModel : ViewModelBase
    {
        readonly IMatchingService _matchingService;

        readonly IEventAggregator _ea;

        readonly IPairsService _pairsService;

        public StandbyMatchingViewModel(IMatchingService matchingService, IEventAggregator ea, IPairsService pairsService)
        {
            _matchingService = matchingService;
            _ea = ea;
            _pairsService = pairsService;
            
            _ea.GetEvent<NewMatchEvent>()
                .Subscribe(standbyPair =>
                {
                    StandbyPairs.Add(standbyPair);
                });
        }

        DelegateCommand _Load;
        public DelegateCommand Load => _Load ??= new(
        async () =>
        {
            await Refresh();
            IsInitialized = true;
        }, () => !IsInitialized || !IsLoaded);

        private async Task Refresh()
        {
            IsLoaded = true;
            var pairs = await _pairsService.GetAllStandbyPairs();

            StandbyPairs.Clear();
            StandbyPairs.AddRange(pairs);

            IsLoaded = true;
        }

        public ObservableCollection<StandbyPair> StandbyPairs { get; set; } = new();

        private StandbyPair _selectedStandbyPair;
        public StandbyPair SelectedStandbyPair
        {
            get => _selectedStandbyPair;
            set => SetProperty(ref _selectedStandbyPair, value);
        }

        DelegateCommand _ActivePairCommand;
        public DelegateCommand ActivePairCommand => _ActivePairCommand ??= new(
        async () =>
        {
            var pair = SelectedStandbyPair.Pair;
            
            StandbyPairs.Remove(SelectedStandbyPair);
            
            var activePair = await _pairsService.ActivePair(pair);
            
            _ea.GetEvent<NewPairEvent>().Publish(activePair);
        });

        DelegateCommand _CancelStandbyPairCommand;
        public DelegateCommand CancelStandbyPairCommand => _CancelStandbyPairCommand ??= new(
        async () =>
        {
            var pair = SelectedStandbyPair.Pair;

            StandbyPairs.Remove(SelectedStandbyPair);
            
            await _pairsService.CancelPair(pair);

            await _matchingService.Refresh();
            _ea.GetEvent<RefreshMatchingEvent>().Publish();
        });
    }
}
