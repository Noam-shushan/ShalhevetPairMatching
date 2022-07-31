using GuiWpf.Events;
using MahApps.Metro.Controls.Dialogs;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using PairMatching.Tools;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : BindableBase
    {
        private const string allYears = "כל השנים";
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly IDialogCoordinator _dialog;

        public ParticipiantsViewModel(IParticipantService participantService, IEventAggregator ea, IDialogCoordinator dialog)
        {
            _participantService = participantService;
            _ea = ea;
            
            Participiants = CollectionViewSource.GetDefaultView(_participiants);
            Participiants.Filter = ParticipiantsFilter;
            Participiants.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Participant.Country)));
            Participiants.SortDescriptions.Add(new SortDescription(nameof(Participant.DateOfRegistered), ListSortDirection.Descending));
            
            SubscribeToEvents();
            _dialog = dialog;
        }

        private bool ParticipiantsFilter(object obj)
        {
            var partKind = false;
            var year = false;
            if (obj is Participant  participant)
            {
                partKind = PartsKindFilter switch
                {
                    ParticipiantsKind.All => true,
                    ParticipiantsKind.WithPair => participant.MatchTo.Any(),
                    ParticipiantsKind.WithoutPair => !participant.MatchTo.Any(),
                    ParticipiantsKind.FromIsrael => participant.IsFromIsrael,
                    ParticipiantsKind.FromWorld => !participant.IsFromIsrael,
                    ParticipiantsKind.FromIsraelWithoutPair => participant.IsFromIsrael && !participant.MatchTo.Any(),
                    ParticipiantsKind.FromWorldWithoutPair => !participant.IsFromIsrael && !participant.MatchTo.Any(),
                    _ => true,
                };

                year = participant.DateOfRegistered.Year.ToString() == YearsFilter || YearsFilter == allYears;

                return participant.Name.Contains(SearchParticipiantsWord, StringComparison.InvariantCultureIgnoreCase)
                    && partKind
                    && year;
            }
            return false;
        }

        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
               async () =>
               {
                   //await MetroProgressOnLoading();
                   IsLoaded = true;

                   //var parts = await _participantService.GetAll();
                   var parts = await _participantService.GetParticipantsWix(); //await _participantService.GetAll();
                   _participiants.Clear();
                   _participiants.AddRange(parts);

                   Years.Clear();
                   Years.AddRange(parts.Select(p => p.DateOfRegistered.Year.ToString()).Distinct());
                   Years.Insert(0, allYears);

                   IsInitialized = true;
                   IsLoaded = false;
               },
               () => !IsInitialized || IsLoaded);

        private bool _isInitialized = false;

        public ObservableCollection<string> Years { get; private set; } = new();

        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(ref _isLoaded, value);
        }

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set 
            { 
                if(SetProperty(ref _isAllSelected, value))
                {
                    Participiants.OfType<Participant>()
                        .ToList()
                        .ForEach(p => p.IsSelected = value);
                    Participiants.Refresh();
                } 
            }
        }

        private void SubscribeToEvents()
        {
            _ea.GetEvent<CloseDialogEvent>().Subscribe(CloseFormResived);
            _ea.GetEvent<AddParticipantEvent>().Subscribe(async (part) =>
            {
                _participiants.Add(part);
                await _participantService.UpserteParticipant(part);
            });
            _ea.GetEvent<CloseDialogEvent>().Subscribe((isClose) =>
            {
                IsSendEmailOpen = isClose;
            });
        }
        
        public ObservableCollection<Participant> _participiants = new();

        public ICollectionView Participiants { get; }

        private Participant _selectedParticipant = new();
        public Participant SelectedParticipant
        {
            get => _selectedParticipant;
            set
            {
                if (SetProperty(ref _selectedParticipant, value))
                {
                    if (_selectedParticipant is not null)
                    {
                        _ea.GetEvent<GetNotesListEvent>()
                        .Publish(SelectedParticipant.Notes);
                        _ea.GetEvent<ModelEnterEvent>()
                            .Publish(ModelType.Participant);
                    }
                };
            }
        }

        private bool _isAddFormOpen = false;
        public bool IsAddFormOpen
        {
            get { return _isAddFormOpen; }
            set { SetProperty(ref _isAddFormOpen, value); }
        }

        DelegateCommand _addParticipantCommand;
        public DelegateCommand AddParticipantCommand => _addParticipantCommand ??= new(
            () =>
            {
                IsAddFormOpen = !IsAddFormOpen;
            });

        private void CloseFormResived(bool isClose)
        {
            IsAddFormOpen = isClose;
        }


        private string _searchParticipiantsWord = "";
        public string SearchParticipiantsWord
        {
            get { return _searchParticipiantsWord; }
            set
            {
                if (SetProperty(ref _searchParticipiantsWord, value))
                {
                    Participiants.Refresh();
                }
            }
        }


        private string _yearsFilter = allYears;
        public string YearsFilter
        {
            get => _yearsFilter;
            set
            {
                if (SetProperty(ref _yearsFilter, value))
                {
                    Participiants.Refresh();
                }
            }
        }

        private ParticipiantsKind _partsKindFilter;
        public ParticipiantsKind PartsKindFilter
        {
            get => _partsKindFilter;
            set
            {
                if (SetProperty(ref _partsKindFilter, value))
                {
                    Participiants.Refresh();
                }
            }
        }

        private bool _isSendEmailOpen = false;
        public bool IsSendEmailOpen
        {
            get => _isSendEmailOpen;
            set => SetProperty(ref _isSendEmailOpen, value);
        }

        DelegateCommand _sendEmailToManyCommand;
        public DelegateCommand SendEmailToManyCommand => _sendEmailToManyCommand ??= new(
        () =>
        {
            IsSendEmailOpen = !IsSendEmailOpen;
            if (IsSendEmailOpen)
            {
                var address = from p in Participiants.OfType<Participant>()
                              where p.IsSelected
                              select new EmailAddress
                              {
                                  Address = p.Email,
                                  Name = p.Name
                              };
                _ea.GetEvent<GetEmailAddressToParticipaintsEvent>()
                .Publish(address);
            }
        });
    }

    public enum ParticipiantsKind
    {
        [EnumDescription("כל המשתתפים")]
        All,

        [EnumDescription("עם חברותא")]
        WithPair,

        [EnumDescription("בלי חברותא")]
        WithoutPair,

        [EnumDescription("מישראל")]
        FromIsrael,

        [EnumDescription("מהתפוצות")]
        FromWorld,

        [EnumDescription("מישראל בלי חברותא")]

        FromIsraelWithoutPair,

        [EnumDescription("מהתפוצות בלי חברותא")]
        FromWorldWithoutPair,
    }
}
