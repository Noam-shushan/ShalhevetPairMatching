using GuiWpf.UIModels;
using PairMatching.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.ViewModels
{
    public class ParticipaintWithSuggestionsViewModel : BindableBase
    {
        public Participant Participant { get; set; }

        List<ParticipantSuggestion> _suggestions;
        public List<ParticipantSuggestion> Suggestions
        {
            get => _suggestions;
            set
            {
                _suggestions = new(value.OrderByDescending(s => s.MatchingPercent));
            }
        }

        private string _searchParticipaintWordForFreeMatch = "";
        public string SearchParticipaintWordForFreeMatch
        {
            get => _searchParticipaintWordForFreeMatch;
            set
            {
                SetProperty(ref _searchParticipaintWordForFreeMatch, value);
                IsFreeMatchSelectionOpen = _searchParticipaintWordForFreeMatch.Length >= 2;
            }
        }

        private bool _isFreeMatchSelectionOpen;
        public bool IsFreeMatchSelectionOpen
        {
            get => _isFreeMatchSelectionOpen;
            set
            {
                if(SetProperty(ref _isFreeMatchSelectionOpen, value))
                {
                    
                }
            }
        }

        //protected void OnPropertyChanged([CallerMemberName] string name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}

        //public event PropertyChangedEventHandler? PropertyChanged;
    }
}
