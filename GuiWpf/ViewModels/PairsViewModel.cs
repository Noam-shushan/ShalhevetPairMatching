using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.Services;
using Prism.Commands;
using System.Collections.ObjectModel;
using PairMatching.Models;
using Prism.Events;
using GuiWpf.Events;

namespace GuiWpf.ViewModels
{
    public class PairsViewModel : BindableBase
    {
        readonly IPairsService _pairsService;
        readonly IEventAggregator _ea;

        public PairsViewModel(IPairsService pairsService, IEventAggregator ea)
        {
            _pairsService = pairsService;
            _ea = ea;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _ea.GetEvent<NewNoteEvent>().Subscribe(NewNoteResivd);
            _ea.GetEvent<DeleteNoteEvent>().Subscribe(OnDeleteNote);
        }

        private void OnDeleteNote(Note note)
        {
            SelectedPair.Notes.Remove(note);
        }

        private void NewNoteResivd(Note note)
        {
            SelectedPair.Notes.Add(note);
        }

        private Pair _selectedPair;
        public Pair SelectedPair
        {
            get => _selectedPair;
            set 
            { 
                if(SetProperty(ref _selectedPair, value))
                {
                    _ea.GetEvent<GetNotesListEvent>()
                        .Publish(SelectedPair.Notes);
                } 
            }
        }

        public ObservableCollection<Pair> Pairs { get; set; } = new();


        DelegateCommand _LoadCommand;
        public DelegateCommand LoadCommand => _LoadCommand ??= new(
        async () =>
        {
            var list = await _pairsService.GetAllPairs();
            Pairs.Clear();
            Pairs.AddRange(list);
        });
    }
}
