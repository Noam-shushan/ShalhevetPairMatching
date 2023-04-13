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
using PairMatching.Tools;
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

        readonly ExceptionHeandler _exceptionHeandler;

        public StandbyMatchingViewModel(IMatchingService matchingService, IEventAggregator ea, IPairsService pairsService, ExceptionHeandler exceptionHeandler)
        {
            _matchingService = matchingService;
            _ea = ea;
            _pairsService = pairsService;
            _exceptionHeandler = exceptionHeandler;
            
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
        }, () => !IsInitialized && !IsLoaded);

        private async Task Refresh()
        {
            try
            {
                IsLoaded = true;
                var pairs = await _pairsService.GetAllStandbyPairs();

                StandbyPairs.Clear();
                StandbyPairs.AddRange(pairs);

                IsLoaded = false;
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        }

        public ObservableCollection<StandbyPair> StandbyPairs { get; set; } = new();

        private StandbyPair _selectedStandbyPair;
        public StandbyPair SelectedStandbyPair
        {
            get => _selectedStandbyPair;
            set => SetProperty(ref _selectedStandbyPair, value);
        }

        DelegateCommand<object> _ChangeTrackCommand;
        public DelegateCommand<object> ChangeTrackCommand => _ChangeTrackCommand ??= new(
        async (track) =>
        {
            if (track is string trackStr)
            {
                var selectedTrack = Extensions.GetValueFromDescription<PrefferdTracks>(trackStr);
                if (SelectedStandbyPair.Pair != null && SelectedStandbyPair.PairSuggestion.ChosenTrack != selectedTrack)
                {
                    if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך לשנות את המסלול ל- {trackStr}"))
                    {
                        try
                        {
                            IsLoaded = true;
                            await _pairsService.ChangeTrack(SelectedStandbyPair.Pair, selectedTrack);
                            SelectedStandbyPair.Pair.Track = selectedTrack;
                            SelectedStandbyPair.PairSuggestion.ChosenTrack = selectedTrack;
                            IsLoaded = false;
                        }
                        catch (Exception ex)
                        {
                            _exceptionHeandler.HeandleException(ex);
                        }
                    }

                }
            }
        }, (obj) => !IsLoaded);

        DelegateCommand _ActivePairCommand;
        public DelegateCommand ActivePairCommand => _ActivePairCommand ??= new(
        async () =>
        {
            try
            {
                if (Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך לתאם סופית את {SelectedStandbyPair.Pair.FromIsrael.Name} ל- {SelectedStandbyPair.Pair.FromWorld.Name} במסלול '{SelectedStandbyPair.PairSuggestion.ChosenTrack.GetDescriptionFromEnumValue()}'"))
                {
                    IsLoaded = true;
                    var pair = SelectedStandbyPair.Pair;

                    StandbyPairs.Remove(SelectedStandbyPair);

                    var activePair = await _pairsService.ActivePair(pair);

                    _ea.GetEvent<NewPairEvent>().Publish(activePair);
                    IsLoaded = false;
                }
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        },
        () => !IsLoaded);

        DelegateCommand _CancelStandbyPairCommand;
        public DelegateCommand CancelStandbyPairCommand => _CancelStandbyPairCommand ??= new(
        async () =>
        {
            try
            {
                IsLoaded = true;
                var pair = SelectedStandbyPair.Pair;

                StandbyPairs.Remove(SelectedStandbyPair);

                await _pairsService.CancelPair(pair);

                await _matchingService.Refresh();
                _ea.GetEvent<RefreshMatchingEvent>().Publish();
                IsLoaded = false;
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        },
        () => !IsLoaded);
    }
}
