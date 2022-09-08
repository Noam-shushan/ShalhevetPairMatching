﻿using GuiWpf.Events;
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
using System.Windows.Data;

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : ViewModelBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly IDialogCoordinator _dialog;

        public ParticipiantsViewModel(IParticipantService participantService, IEventAggregator ea, IDialogCoordinator dialog)
        {
            _participantService = participantService;
            _ea = ea;

            Participiants = new(_ea);
            //Participiants = new PCV<Participant>(_participiants, 20);//CollectionViewSource.GetDefaultView(_participiants);
            //Participiants.Filter = ParticipiantsFilter;
            //Participiants.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Participant.Country)));
            //Participiants.SortDescriptions.Add(new SortDescription(nameof(Participant.DateOfRegistered), ListSortDirection.Descending));

            SubscribeToEvents();

            //Participiants = new(_participiants, 10, ParticipiantsFilter);
            _dialog = dialog;
        }
        

        #region Properties:

        #region Collections
        public ObservableCollection<Participant> _participiants = new();
        
        public PaginCollectionView<Participant> Participiants { get; set; }

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
                   //await MetroProgressOnLoading();
                   await Refresh();
                   IsInitialized = true;
               },
               () => !IsInitialized || IsLoaded);

        DelegateCommand _sendEmailToManyCommand;
        public DelegateCommand SendEmailToManyCommand => _sendEmailToManyCommand ??= new(
        () =>
        {
            IsSendEmailOpen = !IsSendEmailOpen;
            if (IsSendEmailOpen)
            {
                var address = from p in Participiants.Items.OfType<Participant>()
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

        DelegateCommand _addParticipantCommand;
        public DelegateCommand AddParticipantCommand => _addParticipantCommand ??= new(
            () =>
            {
                IsAddFormOpen = !IsAddFormOpen;
            });
        #endregion

        #region Methods
        
        private const string allYears = "כל השנים";

        private async Task Refresh()
        {
            IsLoaded = true;

            var parts = await _participantService.GetAll();
            _participiants.Clear();
            _participiants.AddRange(parts);

            Participiants.Init(_participiants, 10, ParticipiantsFilter);
            
            //Participiants.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Participant.Country)));
            //Participiants.SortDescriptions.Add(new SortDescription(nameof(Participant.DateOfRegistered), ListSortDirection.Descending));

            Years.Clear();
            Years.AddRange(parts.Select(p => p.DateOfRegistered.Year.ToString()).Distinct());
            Years.Insert(0, allYears);

            IsLoaded = false;
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

        private bool ParticipiantsFilter(Participant participant)
        {
            var partKind = false;
            var year = false;

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

        private void CloseFormResived(bool isClose)
        {
            IsAddFormOpen = isClose;
        } 
        #endregion
    }
}