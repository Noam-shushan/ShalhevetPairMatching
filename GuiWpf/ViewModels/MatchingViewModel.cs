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
                   IsLoaded = true;
                   var suggestions = await _matchingService.GetAllPairSuggestions();
                   PairSuggestions.Clear();
                   PairSuggestions.AddRange(suggestions.OrderBy(s => s.MatchingScore));
                   StageSuggestion = PairSuggestions.FirstOrDefault()!;

                   Participants.Clear();
                   Participants.AddRange(from p in suggestions                                        
                                         let ip = p.FromIsrael
                                         group new { p.MatchingScore, p.FromWorld} by new {ip.Id, ip } into ps
                                         select new ParticipaintWithSuggestions
                                         {
                                             Participant = ps.Key.ip,
                                             Suggestions = ps.Select(i => new ParticipantSuggestion 
                                             {
                                                Country = i.FromWorld.Country,
                                                Id = i.FromWorld.Id,
                                                Name = i.FromWorld.Name,
                                                MatchingPercent = Math.Round((double)(100 * i.MatchingScore) / 26)
                                             })
                                         })
                                        .Union(from p in suggestions
                                               let wp = p.FromWorld
                                               group new { p.MatchingScore, p.FromIsrael } by new { wp.Id, wp } into ps
                                               select new ParticipaintWithSuggestions
                                               {
                                                   Participant = ps.Key.wp,
                                                   Suggestions = ps.Select(i => new ParticipantSuggestion
                                                   {
                                                       Country = i.FromIsrael.Country,
                                                       Id = i.FromIsrael.Id,
                                                       Name = i.FromIsrael.Name,
                                                       MatchingPercent = Math.Round((double)(100 * i.MatchingScore) / 26)
                                                   })
                                               });
               
                   IsInitialized = true;
                   IsLoaded = false;

               },
               () => !IsInitialized || IsLoaded);

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


        private ParticipaintWithSuggestions _selectedParticipaint;
        public ParticipaintWithSuggestions SelectedParticipaint
        {
            get => _selectedParticipaint;
            set => SetProperty(ref _selectedParticipaint, value);
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
        public IEnumerable<ParticipantSuggestion> Suggestions { get; set; }
    }
    
    public class ParticipantSuggestion
    {
        public string Id { get; set; }

        public string Country { get; set; }

        public string Name { get; set; }

        public double MatchingPercent { get; set; }
    }
}
