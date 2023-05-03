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
using MailKit;

namespace GuiWpf.ViewModels
{
    public class PairsViewModel : ViewModelBase
    {
        readonly IPairsService _pairsService;
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly ExceptionHeandler _exceptionHeandler;

        public PairsViewModel(IPairsService pairsService, IParticipantService participantService, IEventAggregator ea, ExceptionHeandler exceptionHeandler, IEmailService emailService)
        {
            _pairsService = pairsService;
            _participantService = participantService;
            _ea = ea;
            _exceptionHeandler = exceptionHeandler;

            SubscribeToEvents();
            
            MyNotesViewModel = new NotesViewModel(participantService, pairsService, exceptionHeandler);
            
            SendEmailVmForIsraelParticipaint = 
                new SendEmailViewModel(ea, emailService, exceptionHeandler) { IsLeftToRight = false };
            
            SendEmailVmForWorldParticipaint = 
                new SendEmailViewModel(ea, emailService, exceptionHeandler) { IsLeftToRight = true };
        }

        #region Collections
        public PaginCollectionViewModel<Pair> Pairs { get; set; } = new();

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
                        if (_selectedPair != null)
                        {
                            MyNotesViewModel.Init(_selectedPair);
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties
        private NotesViewModel _myNotesViewModel;
        public NotesViewModel MyNotesViewModel
        {
            get => _myNotesViewModel;
            set => SetProperty(ref _myNotesViewModel, value);
        }

        private SendEmailViewModel _sendEmailVmForIsraelParticipaint;
        public SendEmailViewModel SendEmailVmForIsraelParticipaint
        {
            get => _sendEmailVmForIsraelParticipaint;
            set => SetProperty(ref _sendEmailVmForIsraelParticipaint, value);
        }


        private SendEmailViewModel _sendEmailVmForWorldParticipaint;
        public SendEmailViewModel SendEmailVmForWorldParticipaint
        {
            get => _sendEmailVmForWorldParticipaint;
            set => SetProperty(ref _sendEmailVmForWorldParticipaint, value);
        }


        private bool _isSendEmailOpenIsrael;
        public bool IsSendEmailOpenIsrael
        {
            get => _isSendEmailOpenIsrael;
            set => SetProperty(ref _isSendEmailOpenIsrael, value);
        }


        private bool _isSendEmailOpenWorld;
        public bool IsSendEmailOpenWorld
        {
            get => _isSendEmailOpenWorld;
            set => SetProperty(ref _isSendEmailOpenWorld, value);
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
                    Pairs.FilterdItems
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

        DelegateCommand<object> _ChangeTrackCommand;
        public DelegateCommand<object> ChangeTrackCommand => _ChangeTrackCommand ??= new(
        async (track) =>
        {
            if(track is string trackStr)
            {
                var selectedTrack = Extensions.GetValueFromDescription<PrefferdTracks>(trackStr);
                if (SelectedPair != null && SelectedPair.Track != selectedTrack)
                {
                    if(Messages.MessageBoxConfirmation($"האם אתה בטוח שברצונך לשנות את המסלול ל- {trackStr}"))
                    {
                        try
                        {
                            IsLoaded = true;
                            await _pairsService.ChangeTrack(SelectedPair, selectedTrack);
                            SelectedPair.Track = selectedTrack;
                            IsLoaded = false;
                        }
                        catch (Exception ex)
                        {
                            _exceptionHeandler.HeandleException(ex);
                        }
                    }

                }
            }
        }, (_) => !IsLoaded);

        
        DelegateCommand<object> _ChangeStatusCommand;
        public DelegateCommand<object> ChangeStatusCommand => _ChangeStatusCommand ??= new(
        async (statusStr) =>
        {
            if (statusStr is string status)
            {
                var selectedStatus = Extensions.GetValueFromDescription<PairStatus>(status);
                if (SelectedPair != null && SelectedPair.Status != selectedStatus)
                {
                    try
                    {
                        IsLoaded = true;
                        await _pairsService.ChangeStatus(SelectedPair, selectedStatus);
                        SelectedPair.Status = selectedStatus;
                        IsLoaded = false;
                    }
                    catch (Exception ex)
                    {
                        _exceptionHeandler.HeandleException(ex);
                    }
                }
            }
        },(_) => !IsLoaded);

        DelegateCommand _DeleteAllPairsCommand;
        public DelegateCommand DeleteAllPairsCommand => _DeleteAllPairsCommand ??= new(
        async () =>
        {
            if(Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך למחוק את כל הנבחרים?"))
            {
                try
                {
                    var pairsToDel = Pairs.FilterdItems.Where(p => p.IsSelected);
                    List<Task> tasks = new();
                    foreach (var p in pairsToDel)
                    {
                        tasks.Add(_pairsService.DeletePair(p));
                    }
                    await Task.WhenAll(tasks);
                    foreach (var p in pairsToDel)
                    {
                        Pairs.ItemsSource.Remove(p);
                    }
                    Pairs.Refresh();
                }
                catch (Exception ex)
                {
                    _exceptionHeandler.HeandleException(ex);
                }
            }
        }, () => !IsLoaded);

        DelegateCommand _DeletePairCommand;
        public DelegateCommand DeletePairCommand => _DeletePairCommand ??= new(
        async () =>
        {
            if (SelectedPair != null)
            {
                if(Messages.MessageBoxConfirmation("בטוח שברצונך למחוק את החברותא?"))
                {
                    try
                    {
                        IsLoaded = true;
                        await _pairsService.DeletePair(SelectedPair);
                        Pairs.ItemsSource.Remove(SelectedPair);
                        Pairs.Refresh();
                        IsLoaded = false;
                    }
                    catch (Exception ex)
                    {
                        _exceptionHeandler.HeandleException(ex);
                    }
                }
            }
        }, ()=> !IsLoaded);

        DelegateCommand _ClearFilterCommand;
        public DelegateCommand ClearFilterCommand => _ClearFilterCommand ??= new(
        () =>
        {
            PairKindFilter = PairKind.All;
            TrackFilter = allTracks;
            YearsFilter = allYears;
            SearchPairsWord = "";
        });


        DelegateCommand _SendEmailToManyCommand;
        public DelegateCommand SendEmailToManyCommand => _SendEmailToManyCommand ??= new(
        () =>
        {
            var ipAdresses = from pair in Pairs.FilterdItems
                             where pair.IsSelected
                             select new EmailAddress
                             {
                                 Address = pair.FromIsrael.Email,
                                 Name = pair.FromIsrael.Name,
                                 ParticipantId = pair.FromIsrael.Id,
                                 ParticipantWixId = pair.FromIsrael.WixId,
                             };
            SendEmailVmForIsraelParticipaint.Init(ipAdresses, true);

            var wpAdresses = from pair in Pairs.FilterdItems
                             where pair.IsSelected
                             select new EmailAddress
                             {
                                 Address = pair.FromWorld.Email,
                                 Name = pair.FromWorld.Name,
                                 ParticipantId = pair.FromWorld.Id,
                                 ParticipantWixId = pair.FromWorld.WixId,
                             };
            SendEmailVmForWorldParticipaint.Init(wpAdresses, true);
        });

        DelegateCommand _SendToOneEmailCommand;
        public DelegateCommand SendEmailToOneCommand => _SendToOneEmailCommand ??= new(
        () =>
        {
            SendEmailVmForIsraelParticipaint.Init(
                new EmailAddress[]
                {
                    new EmailAddress()
                    {
                        Address = SelectedPair.FromIsrael.Email,
                        Name = SelectedPair.FromIsrael.Name,
                        ParticipantId = SelectedPair.FromIsrael.Id,
                        ParticipantWixId = SelectedPair.FromIsrael.WixId,
                    }
                }, true);
            
            SendEmailVmForWorldParticipaint.Init(
                new EmailAddress[]
                {
                            new EmailAddress()
                            {
                                Address = SelectedPair.FromWorld.Email,
                                Name = SelectedPair.FromWorld.Name,
                                ParticipantId = SelectedPair.FromWorld.Id,
                                ParticipantWixId = SelectedPair.FromWorld.WixId,
                            }
                }, true);
        });
        #endregion

        #region Mathods
        private async Task Refresh()
        {
            try
            {
                IsLoaded = true;

                await _pairsService.VerifieyNewPairsInWix();

                var pairs = await _pairsService.GetAllPairs();
                Pairs.Init(pairs.OrderByDescending(p => p.DateOfCreate), 10, PairsFilter);

                Years.Clear();
                Years.AddRange(pairs.Select(p => p.DateOfCreate.Year.ToString()).Distinct());
                Years.Insert(0, allYears);

                IsLoaded = false;
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
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
            return pair.FromIsrael.Name.SearchText(SearchPairsWord)
                || pair.FromWorld.Name.SearchText(SearchPairsWord);
        }

        private void SubscribeToEvents()
        {   
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
