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
                .Subscribe(async ps =>
                {
                    await Refresh();
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

        private Pair _selectedStandbyPair;
        public Pair SelectedStandbyPair
        {
            get => _selectedStandbyPair;
            set => SetProperty(ref _selectedStandbyPair, value);
        }
    }
}
