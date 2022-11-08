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
using GuiWpf.UIModels;
using System.Diagnostics;
using PairMatching.Tools;
using System.Windows.Controls.Primitives;

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

        #region Collections
        public PaginCollectionViewModel<Pair> Pairs { get; set; } = new();

        ObservableCollection<Pair> _pairs { get; set; } = new();

        public ObservableCollection<string> Years { get; private set; } = new();

        public ObservableCollection<string> Tracks { get; private set; } = new();
        #endregion

        #region Selections
        private Pair _selectedPair = new();
        public Pair SelectedPair
        {
            get => _selectedPair;
            set
            {
                if (SetProperty(ref _selectedPair, value))
                {
                    if (_selectedPair != null)
                    {
                        _ea.GetEvent<ManageNotesForPairEvent>()
                            .Publish(SelectedPair);
                        //_ea.GetEvent<GetNotesListEvent>()
                        //    .Publish(SelectedPair.Notes);
                        //_ea.GetEvent<ModelEnterEvent>()
                        //    .Publish((SelectedPair, ModelType.Participant));
                    }
                }
            }
        } 
        #endregion

        #region Filtering
        private PairKind _pairKindFilter = PairKind.All;
        public PairKind PairKindFilter
        {
            get => _pairKindFilter;
            set
            {
                if (SetProperty(ref _pairKindFilter, value))
                {
                    Pairs.Refresh();
                }
            }
        }

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (SetProperty(ref _isAllSelected, value))
                {
                    Pairs.Items.OfType<Participant>()
                        .ToList()
                        .ForEach(p => p.IsSelected = value);
                    Pairs.Refresh();
                }
            }
        }

        private const string allYears = "כל השנים";
        private const string allTracks = "כל המסלולים";

        private string _yearsFilter = allYears;
        public string YearsFilter
        {
            get => _yearsFilter;
            set
            {
                if (SetProperty(ref _yearsFilter, value))
                {
                    Pairs.Refresh();
                }
            }
        }

        private string _searchPairsWord = "";
        public string SearchPairsWord
        {
            get { return _searchPairsWord; }
            set
            {
                if (SetProperty(ref _searchPairsWord, value))
                {
                    Pairs.Refresh();
                }
            }
        }

        private string _trackFilter = allTracks;
        public string TrackFilter
        {
            get { return _trackFilter; }
            set
            {
                if (SetProperty(ref _trackFilter, value))
                {
                    Pairs.Refresh();
                }
            }
        }
        #endregion

        #region Commands
        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
        async () =>
        {
            Tracks.Clear();
            Tracks.AddRange(from e in Enum.GetValues(typeof(PrefferdTracks)).Cast<Enum>()
                            let enumStringValue = e.GetDescriptionFromEnumValue()
                            select enumStringValue);
            Tracks.Insert(0, allTracks);

            await Refresh();
            IsInitialized = true;
        },
        () => !IsInitialized && !IsLoaded);

        DelegateCommand _ClearFilterCommand;
        public DelegateCommand ClearFilterCommand => _ClearFilterCommand ??= new(
        () =>
        {
            PairKindFilter = PairKind.All;
            TrackFilter = allTracks;
            YearsFilter = allYears;
        });
        #endregion

        #region Mathods
        private async Task Refresh()
        {
            IsLoaded = true;

            var list = await _pairsService.GetAllPairs();
            _pairs.Clear();
            var l = from p in list
            orderby p.Notes.Count
            select p;
            _pairs.AddRange(list);
            Pairs.Init(_pairs, 5, PairsFilter);

            Years.Clear();
            Years.AddRange(_pairs.Select(p => p.DateOfCreate.Year.ToString()).Distinct());
            Years.Insert(0, allYears);

            IsLoaded = false;
        }

        bool PairsFilter(Pair pair)
        {
            var pairKind = PairKindFilter switch
            {
                PairKind.All => true,
                PairKind.Active => pair.IsActive,
                PairKind.Inactive => !pair.IsActive,
                PairKind.Learning => pair.Status == PairStatus.Learning,
                _ => true
            };

            var track = Extensions.GetValueFromDescription<PrefferdTracks>(TrackFilter) == pair.Track
                || TrackFilter == allTracks;

            var year = pair.DateOfCreate.Year.ToString() == YearsFilter || YearsFilter == allYears;

            return SearchPair(pair) &&
                track &&
                pairKind &&
                year;
        }

        bool SearchPair(Pair pair)
        {
            return pair.FromIsrael.Name.Contains(SearchPairsWord, StringComparison.InvariantCultureIgnoreCase)
                || pair.FromWorld.Name.Contains(SearchPairsWord, StringComparison.InvariantCultureIgnoreCase);
        }

        private void SubscribeToEvents()
        {
            //_ea.GetEvent<NewNoteForPairEvent>().Subscribe((id_note) =>
            //{
            //    var pair = Pairs.ItemsSource.FirstOrDefault(p => p.Id == id_note.Item1);
            //    if (pair == null)
            //    {
            //        return;
            //    }
            //    pair.Notes.Add(id_note.Item2);
            //    _pairsService.UpdatePair(pair);
            //});
            
            //_ea.GetEvent<DeleteNoteFromPairEvent>().Subscribe((id_note) =>
            //{
            //    var pair = Pairs.ItemsSource.FirstOrDefault(p => p.Id == id_note.Item1);
            //    if (pair == null)
            //    {
            //        return;
            //    }
            //    pair.Notes.Remove(id_note.Item2);
            //    _pairsService.UpdatePair(pair);
            //});
            
            _ea.GetEvent<NewPairEvent>().Subscribe((pair) =>
            {
                if (pair is not null)
                {
                    Pairs.Add(pair);
                }
            });
        }
        #endregion
    }
}
