using GuiWpf.Commands;
using GuiWpf.Events;
using GuiWpf.UIModels;
using MahApps.Metro.Controls.Dialogs;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using PairMatching.ExcelTool;
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
using GuiWpf.Views;

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : ViewModelBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly ExceptionHeandler _exceptionHeandler;

        public ParticipiantsViewModel(IParticipantService participantService, IPairsService pairService, IEmailService emailService, ExcelExportingService excel, IEventAggregator ea, ExceptionHeandler exceptionHeandler)
        {
            _participantService = participantService;
            _ea = ea;
            _exceptionHeandler = exceptionHeandler;

            SubscribeToEvents();

            MyNotesViewModel = new NotesViewModel(participantService, pairService, exceptionHeandler);
            SendEmailVm = new SendEmailViewModel(ea, emailService, exceptionHeandler);
            ExportToExcelVm = new(excel);
            FullParticipaintVm = new();
            EditParticipaintVm = new EditParticipaintViewModel(participantService, ea, exceptionHeandler);
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

        private SendEmailViewModel _sendEmailVm;
        public SendEmailViewModel SendEmailVm
        {
            get => _sendEmailVm;
            set => SetProperty(ref _sendEmailVm, value);
        }
        #endregion

        #region Navigation Properties
        private bool _isAddFormOpen = false;
        public bool IsAddFormOpen
        {
            get { return _isAddFormOpen; }
            set { SetProperty(ref _isAddFormOpen, value); }
        }

        private bool _isEditParticipaintOpen;
        public bool IsEditParticipaintOpen
        {
            get => _isEditParticipaintOpen;
            set => SetProperty(ref _isEditParticipaintOpen, value);
        }

        public ExcelExportViewModel<Participant> ExportToExcelVm { get; set; }

        public FullParticipaintViewModel FullParticipaintVm { get; set; }

        public EditParticipaintViewModel EditParticipaintVm { get; set; }

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
                    Participiants.FilterdItems
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
            var address = from p in Participiants.FilterdItems
                          where p.IsSelected
                          select new EmailAddress
                          {
                              Address = p.Email,
                              Name = p.Name,
                              ParticipantId = p.Id,
                              ParticipantWixId = p.WixId
                          };
            SendEmailVm.Init(address, true);

        });

        DelegateCommand _sendEmailToOneCommand;
        public DelegateCommand SendEmailToOneCommand => _sendEmailToOneCommand ??= new(
        () =>
        {
            if (SelectedParticipant == null) return;
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
            SendEmailVm.Init(address, true);

        });


        DelegateCommand<Participant> _ParticipatintSelectedCommand;
        public DelegateCommand<Participant> ParticipatintSelectedCommand => _ParticipatintSelectedCommand ??= new(
        (Participant part) =>
        {
            part.IsSelected = true;
            Participiants.Update(part, inPlace: true);
        });

        DelegateCommand<Participant> _ParticipatintUnselectedCommand;
        public DelegateCommand<Participant> ParticipatintUnselectedCommand => _ParticipatintUnselectedCommand ??= new(
        (Participant part) =>
        {
            part.IsSelected = false;
            Participiants.Update(part, inPlace: true);
        });


        DelegateCommand _DeleteManyCommand;
        public DelegateCommand DeleteManyCommand => _DeleteManyCommand ??= new(
        async () =>
        {
            if (!Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך למחוק את המשתתפים המסומנים?")) return;
            
            var participants = from p in Participiants.ItemsSource
                          where p.IsSelected
                          select p;
            
            if(participants.Count() == 0)
            {
                Messages.MessageBoxSimple("לא נבחרו אף משתתפים");
                return;
            }
            IsLoaded = true;
            await _participantService.DeleteMany(participants);

            await RefreshData();
            IsLoaded = false;
        }, () => !IsLoaded);


        DelegateCommand _SendToArchivCommand;
        public DelegateCommand SendToArchivCommand => _SendToArchivCommand ??= new(
        async () =>
        {
            if (SelectedParticipant == null) return;

            if (!Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך לשלוח את {SelectedParticipant.Name} לארכיון?")) return;
            
            if (SelectedParticipant.IsMatch)
            {
                if (!Messages.MessageBoxConfirmation($"{SelectedParticipant.Name} נמצא בחברותא. שליחה לארכיון תבטל את החברותא. האם אתה בטוח שברצונך להמשיך?"))
                {
                    return;
                }
            }
                
            try
            {
                IsLoaded = true;
                var archivePart = new Participant();
                if(SelectedParticipant is IsraelParticipant ip)
                {
                    archivePart = ip.CopyPropertiesToNew<Participant, IsraelParticipant>();
                }
                if (SelectedParticipant is WorldParticipant wp)
                {
                    archivePart = wp.CopyPropertiesToNew<Participant, WorldParticipant>();
                }
                await _participantService.SendToArcive(archivePart);
                Participiants.Remove(archivePart);             
                _ea.GetEvent<SendToArciveEvent>().Publish(archivePart);

                _ea.GetEvent<RefreshMatchingEvent>().Publish();
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
            finally
            {
                IsLoaded = false;
            }

        }, () => !IsLoaded);
            

        DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand => _DeleteCommand ??= new(
        async () =>
        {
            if(SelectedParticipant == null) return;

            var msg = SelectedParticipant.IsMatch ?
            $"ל {SelectedParticipant.Name} יש חברותא, האם בכל זאת למחוק?"
            : $"האם אתה בטוח שברצונך למחוק את {SelectedParticipant.Name} ?";
            if (!Messages.MessageBoxConfirmation(msg)) return;

            try
            {
                IsLoaded = true;
                await _participantService.DeleteParticipaint(SelectedParticipant.Clone());
                Participiants.Remove(SelectedParticipant.Clone());
                _ea.GetEvent<RefreshMatchingEvent>().Publish();
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
            finally
            {
                IsLoaded = false;
            }
        }, () => !IsLoaded);

        DelegateCommand _addParticipantCommand;
        public DelegateCommand AddParticipantCommand => _addParticipantCommand ??= new(
            () =>
            {
                _ea.GetEvent<NewParticipaintEvent>().Publish(true);
                IsAddFormOpen = !IsAddFormOpen;
            });


        DelegateCommand _OpenEditParticipiantCommand;
        public DelegateCommand OpenEditParticipiantCommand => _OpenEditParticipiantCommand ??= new(
        () =>
        {
            if (SelectedParticipant == null) return;
            EditParticipaintVm.Init(GetSelected()!, true);
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


        DelegateCommand _OpenFullParticipiantCommand;
        public DelegateCommand OpenFullParticipiantCommand => _OpenFullParticipiantCommand ??= new(
        () =>
        {
            if (SelectedParticipant == null) return;
            FullParticipaintVm.Init(GetSelected()!, true);
        });

        public CopyCommand CopyCommand { get; set; } = new();


        DelegateCommand _ExportExcelCommand;
        public DelegateCommand ExportExcelCommand => _ExportExcelCommand ??= new(
        () =>
        {
            var participiants = Participiants.FilterdItems.ToList();
            ExportToExcelVm.Init(participiants, true);
        });
        #endregion

        #region Methods

        private async Task Refresh()
        {
            try
            {
                IsLoaded = true;

                await _participantService.SetNewParticipintsFromWix();
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
            finally
            {
                IsLoaded = false;
            }
            try
            {
                IsLoaded = true;

                var parts = await _participantService.GetAll();

                _ea.GetEvent<ResiveParticipantsEvent>().Publish(parts);

                Years.Clear();
                Years.AddRange(parts.Select(p => p.DateOfRegistered.Year.ToString()).Distinct());
                Years.Insert(0, allYears);

                Participiants.Init(parts.OrderByDescending(p => p.DateOfRegistered), 10, ParticipiantsFilter);
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
            finally
            {
                IsLoaded = false;
            }
        }

        async Task RefreshData()
        {
            try
            {
                IsLoaded = true;

                var parts = await _participantService.GetAll();

                _ea.GetEvent<ResiveParticipantsEvent>().Publish(parts);

                Years.Clear();
                Years.AddRange(parts.Select(p => p.DateOfRegistered.Year.ToString()).Distinct());
                Years.Insert(0, allYears);

                Participiants.Init(parts.OrderByDescending(p => p.DateOfRegistered), 10, new Predicate<Participant>(_ => true));
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
            finally
            {
                IsLoaded = false;
            }
        }

        private void SubscribeToEvents()
        {
            _ea.GetEvent<CloseDialogEvent>().Subscribe((isClose) => 
            {
                IsAddFormOpen = isClose;
            });

            _ea.GetEvent<ParticipaintWesUpdate>()
                .Subscribe(async updetetdParts =>
                {
                    Participiants.Update(updetetdParts);
                });

            _ea.GetEvent<AddParticipantEvent>().Subscribe((part) =>
            {
                Participiants.Add(part);
            });
            _ea.GetEvent<RefreshAll>()
                .Subscribe(async () =>
                {
                    await Refresh();
                });
            _ea.GetEvent<ExloadeFromArciveEvent>()
                .Subscribe(part =>
                {
                    Participiants.Add(part);
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
                _ => true
            };

            year = participant.DateOfRegistered.Year.ToString() == YearsFilter || YearsFilter == allYears;

            return (participant.Name.SearchText(SearchParticipiantsWord) || participant.Email.SearchText(SearchParticipiantsWord))
                && partKind
                && year
                && fromIsrael;
        }

        Participant? GetSelected()
        {
            if (SelectedParticipant == null) return null;
            if(SelectedParticipant is IsraelParticipant ip)
            {
                return ip.Clone();
            }
            if (SelectedParticipant is WorldParticipant wp)
            {
                return wp.Clone();
            }
            return null;
        }


     
        #endregion
    }
}