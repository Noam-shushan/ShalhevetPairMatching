using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace GuiWpf.ViewModels
{
    public class MatchingViewModel : BindableBase
    {
        readonly IMatchingService _matchingService;

        public MatchingViewModel(IMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
               async () =>
               {
                   //await MetroProgressOnLoading();
                   if (IsInitialized)
                   {
                       return;
                   }
                   IsLoaded = true;
                   var suggestions = await _matchingService.GetAllPairSuggestions();
                   PairSuggestions.Clear();
                   PairSuggestions.AddRange(suggestions.OrderBy(s => s.MatchingScore));
                   StageSuggestion = PairSuggestions.FirstOrDefault();
                   
                   IsInitialized = true;
                   IsLoaded = false;
               });

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(ref _isLoaded, value);
        }

        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        public ObservableCollection<PairSuggestion> PairSuggestions { get; set; } = new();
        public ObservableCollection<ParticipaintWithSuggestions> Participants { get; set; } = new();


        PairSuggestion _stageSuggestion;
        public PairSuggestion StageSuggestion
        {
            get => _stageSuggestion;
            set
            {
                SetProperty(ref _stageSuggestion, value);
            }
        }

        DelegateCommand _stageNext;
        public DelegateCommand StageNext => _stageNext ??= new(
               () =>
               {
                   if (StageSuggestion == null)
                   {
                       return;
                   }
                   var index = PairSuggestions.IndexOf(StageSuggestion);
                   index = index + 1 < PairSuggestions.Count ? index + 1 : 0;
                   StageSuggestion = PairSuggestions[index + 1];
               });

        DelegateCommand _stagePrevious;
        public DelegateCommand StagePrevious => _stagePrevious ??= new(
               () =>
               {
                   if (StageSuggestion == null)
                   {
                       return;
                   }
                   var index = PairSuggestions.IndexOf(StageSuggestion);
                   index = index - 1 > 0 ? index - 1 : PairSuggestions.Count - 1;
                   StageSuggestion = PairSuggestions[index];
               });
    }

    public class ParticipaintWithSuggestions
    {
        public Participant Participant { get; set; }
        public List<PairSuggestion> Suggestions { get; set; }
    }
}
