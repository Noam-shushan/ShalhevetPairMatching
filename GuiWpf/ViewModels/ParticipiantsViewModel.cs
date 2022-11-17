using GuiWpf.Events;
using GuiWpf.UIModels;
using MahApps.Metro.Controls.Dialogs;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using PairMatching.Tools;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : ViewModelBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;

        public ParticipiantsViewModel(IParticipantService participantService, IPairsService pairService,  IEventAggregator ea)
        {
            _participantService = participantService;
            _ea = ea;

            SubscribeToEvents();

            MyNotesViewModel = new NotesViewModel(participantService, pairService);
        }
        
        #region Properties:

        #region Collections

        public PaginCollectionViewModel<Participant> Participiants { get; set; } = new();

        public ObservableCollection<string> Years { get; private set; } = new();
        #endregion

        #region Selctions
        private Participant _selectedParticipant = new();
        public Participant SelectedParticipant
        {
            get => _selectedParticipant;
            set
            {
                if (SetProperty(ref _selectedParticipant, value))
                {
                    if (_selectedParticipant != null)
                    {
                        MyNotesViewModel.Init(_selectedParticipant);
                    }
                };
            }
        }
        
        private NotesViewModel _myNotesViewModel;
        public NotesViewModel MyNotesViewModel
        {
            get => _myNotesViewModel;
            set => SetProperty(ref _myNotesViewModel, value);
        }
        #endregion

        #region Navigation Properties
        private bool _isAddFormOpen = false;
        public bool IsAddFormOpen
        {
            get { return _isAddFormOpen; }
            set { SetProperty(ref _isAddFormOpen, value); }
        }

        private bool _isSendEmailOpen = false;
        public bool IsSendEmailOpen
        {
            get => _isSendEmailOpen;
            set => SetProperty(ref _isSendEmailOpen, value);
        }
        #endregion

        #region Filtering
        private const string allYears = "כל השנים";

        private string _yearsFilter = allYears;
        public string YearsFilter
        {
            get => _yearsFilter;
            set
            {
                if (SetProperty(ref _yearsFilter, value))
                {
                    //_ea.GetEvent<RefreshItemsEvnet>()
                    //    .Publish(true);
                    Participiants.Refresh();
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
                    Participiants.Items.OfType<Participant>()
                        .ToList()
                        .ForEach(p => p.IsSelected = value);
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

        private ParticipiantsFrom _fromFilter;
        public ParticipiantsFrom FromIsraelFilter
        {
            get => _fromFilter;
            set
            {
                if (SetProperty(ref _fromFilter, value))
                {
                    Participiants.Refresh();
                }
            }
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
        #endregion

        #endregion

        #region Commands
        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
               async () =>
               {
                   await Refresh();
                   
                   IsInitialized = true;
               },
               () => !IsInitialized && !IsLoaded);

        DelegateCommand _sendEmailToManyCommand;
        public DelegateCommand SendEmailToManyCommand => _sendEmailToManyCommand ??= new(
        () =>
        {
            IsSendEmailOpen = !IsSendEmailOpen;
            if (IsSendEmailOpen)
            {
                var address = from p in Participiants.Items
                              where p.IsSelected
                              select new EmailAddress
                              {
                                  Address = p.Email,
                                  Name = p.Name,
                                  ParticipantId = p.Id,
                                  ParticipantWixId = p.WixId
                              };
                _ea.GetEvent<GetEmailAddressToParticipaintsEvent>()
                .Publish(address);
            }
        });

        DelegateCommand _sendEmailToOneCommand;
        public DelegateCommand SendEmailToOneCommand => _sendEmailToOneCommand ??= new(
        () =>
        {
            IsSendEmailOpen = !IsSendEmailOpen;
            if (IsSendEmailOpen)
            {
                var address = new EmailAddress[]
                {
                    new EmailAddress
                    {
                        Address = SelectedParticipant.Email,
                        Name = SelectedParticipant.Name,
                        ParticipantId = SelectedParticipant.Id,
                        ParticipantWixId = SelectedParticipant.WixId
                    }
                };
                _ea.GetEvent<GetEmailAddressToParticipaintsEvent>()
                .Publish(address);
            }
        });


        DelegateCommand _SendToArchivCommand;
        public DelegateCommand SendToArchivCommand => _SendToArchivCommand ??= new(
        async () =>
        {
            IsLoaded = true;
            await _participantService.SendToArcive(SelectedParticipant);
            SelectedParticipant.IsInArchive = true;
            Participiants.Refresh();
            IsLoaded = false;
        }, () => !IsLoaded);


        DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand => _DeleteCommand ??= new(
        async () =>
        {
            IsLoaded = true;
            await _participantService.DeleteParticipaint(SelectedParticipant);
            Participiants.ItemsSource.Remove(SelectedParticipant);
            Participiants.Refresh();
            IsLoaded = false;
        }, () => !IsLoaded);

        DelegateCommand _addParticipantCommand;
        public DelegateCommand AddParticipantCommand => _addParticipantCommand ??= new(
            () =>
            {

                IsAddFormOpen = !IsAddFormOpen;
            });


        DelegateCommand _ClearFilterCommand;
        public DelegateCommand ClearFilterCommand => _ClearFilterCommand ??= new(
        () =>
        {
            FromIsraelFilter = ParticipiantsFrom.All;
            PartsKindFilter = ParticipiantsKind.All;
            YearsFilter = allYears;
            SearchParticipiantsWord = ""; 
        });
        #endregion

        #region Methods

        private async Task Refresh()
        {
            IsLoaded = true;

            var parts = await _participantService.GetAll();

            Participiants.Init(parts, 10, ParticipiantsFilter);

            

            Years.Clear();
            Years.AddRange(parts.Select(p => p.DateOfRegistered.Year.ToString()).Distinct());
            Years.Insert(0, allYears);

            IsLoaded = false;
        }

        private void SubscribeToEvents()
        {
            _ea.GetEvent<CloseDialogEvent>().Subscribe((isClose) => IsAddFormOpen = isClose);

            _ea.GetEvent<CloseDialogEvent>().Subscribe((isClose) => IsSendEmailOpen = isClose);

            _ea.GetEvent<AddParticipantEvent>().Subscribe(async (part) =>
            {
                var newParticipaint = await _participantService.InsertParticipant(part);
                Participiants.Add(newParticipaint);
            });       
        }

        private bool ParticipiantsFilter(Participant participant)
        {
            var partKind = false;
            var year = false;
            
            var fromIsrael = (participant.IsFromIsrael && FromIsraelFilter == ParticipiantsFrom.FromIsrael)
                || (!participant.IsFromIsrael && FromIsraelFilter == ParticipiantsFrom.FromWorld)
                || ParticipiantsFrom.All == FromIsraelFilter;

            partKind = PartsKindFilter switch
            {
                ParticipiantsKind.All => true,
                ParticipiantsKind.WithPair => participant.MatchTo.Any(),
                ParticipiantsKind.WithoutPair => !participant.MatchTo.Any(),
                ParticipiantsKind.Archive => participant.IsInArchive,
                _ => true
            };

            year = participant.DateOfRegistered.Year.ToString() == YearsFilter || YearsFilter == allYears;

            return participant.Name.Contains(SearchParticipiantsWord, StringComparison.InvariantCultureIgnoreCase)
                && partKind
                && year
                && fromIsrael;
        }
        #endregion
    }
}