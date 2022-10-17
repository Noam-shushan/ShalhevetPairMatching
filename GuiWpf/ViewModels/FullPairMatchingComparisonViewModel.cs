using GuiWpf.Events;
using PairMatching.DomainModel.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.MatchingCalculations;
using Prism.Commands;
using System.Collections.ObjectModel;
using GuiWpf.Commands;
using PairMatching.Models;

namespace GuiWpf.ViewModels
{
    public class FullPairMatchingComparisonViewModel : ViewModelBase
    {
        readonly IEventAggregator _ea;

        public FullPairMatchingComparisonViewModel(IEventAggregator ea, MatchCommand matchCommand)
        {
            _ea = ea;
            Match = matchCommand;

            _ea.GetEvent<ShowFullComparisonEvent>()
                .Subscribe(ps =>
                {
                    if (ps.Item1 is null || ps.Item2 is null)
                        return;
                    
                    PairSuggestions.Clear();
                    PairSuggestions.AddRange(ps.Item1);
                    StageSuggestion = PairSuggestions.FirstOrDefault(p => p.FromIsrael.Id == ps.Item2 
                    || p.FromWorld.Id == ps.Item2)!;
                });
        }
        
        public MatchCommand Match { get; }        

        public ObservableCollection<PairSuggestion> PairSuggestions { get; set; } = new();

        private PairSuggestion _stageSuggestion;
        public PairSuggestion StageSuggestion
        {
            get => _stageSuggestion;
            set => SetProperty(ref _stageSuggestion, value);
        }

        private PrefferdTracks _selectedTrack;
        public PrefferdTracks SelectedTrack
        {
            get => _selectedTrack;
            set => SetProperty(ref _selectedTrack, value);
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
                   StageSuggestion = PairSuggestions[index];
               }
               //, () => PairSuggestions.IndexOf(StageSuggestion) + 1 < PairSuggestions.Count
               );

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
               }
               //, () => PairSuggestions.IndexOf(StageSuggestion) - 1 >= 0
               );

        


    }
}
