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
using GuiWpf.Commands;

namespace GuiWpf.ViewModels
{
    public class AutoMatchingViewModel : ViewModelBase
    {
        readonly IMatchingService _matchingService;

        readonly IEventAggregator _ea;

        readonly ExceptionHeandler _exceptionHeandler;

        public AutoMatchingViewModel(IMatchingService matchingService, IEventAggregator ea, ExceptionHeandler exceptionHeandler)
        {
            _matchingService = matchingService;
            _ea = ea;
            _exceptionHeandler = exceptionHeandler;
            
            _ea.GetEvent<RefreshMatchingEvent>()
                .Subscribe(async () =>
                {
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
            try
            {
                IsLoaded = true;

                var result = await _matchingService.GetMaxOptMatching();
                AutoSuggestions.Clear();
                AutoSuggestions.AddRange(result);

                IsLoaded = false;
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
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
            _ea.GetEvent<ShowFullComparisonEvent>().Publish((AutoSuggestions, SelectedSuggestions.FromIsrael.Id));
            _ea.GetEvent<OpenCloseDialogEvent>().Publish((true, typeof(FullPairMatchingComparisonViewModel)));
        }, () => !IsLoaded);
    }
}
