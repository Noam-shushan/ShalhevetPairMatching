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
    public class PairsViewModel : ViewModelBase
    {
        readonly IPairsService _pairsService;
        readonly IEventAggregator _ea;

        public PairsViewModel(IPairsService pairsService, IEventAggregator ea)
        {
            _pairsService = pairsService;
            _ea = ea;

            SubscribeToEvents();
            
        }

        public PaginCollectionView<Pair> Pairs { get; set; } = new();

        //public ObservableCollection<Pair> Pairs { get; set; } = new();


        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
        async () =>
        {
            IsLoaded = true;
            var list = await _pairsService.GetAllPairs();
            //Pairs.Clear();
            //Pairs.AddRange(list);
            Pairs.Init(list, 5);
            Pairs.Refresh();
            IsLoaded = false;
            IsInitialized = true;
        }, () => !IsInitialized && !IsLoaded);

        private void SubscribeToEvents()
        {
            _ea.GetEvent<NewNoteForPairEvent>().Subscribe(NewNoteResivd);
            _ea.GetEvent<DeleteNoteFromPairEvent>().Subscribe(OnDeleteNote);
            _ea.GetEvent<NewPairEvent>().Subscribe(NewPairResivd);
        }

        private void NewPairResivd(Pair pair)
        {
            if(pair is not null)
            {
                Pairs.ItemsSource.Add(pair);
            }      
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
                    _ea.GetEvent<ModelEnterEvent>()
                        .Publish(ModelType.Participant);
                } 
            }
        }


        DelegateCommand _NextPageCommand;
        public DelegateCommand NextPageCommand => _NextPageCommand ??= new(
        () =>
        {
            Pairs.MoveToNextPage();
        });


        DelegateCommand _PrevPageCommand;
        public DelegateCommand PrevPageCommand => _PrevPageCommand ??= new(
        () =>
        {
            Pairs.MoveToPreviousPage();
        });
    }
}
