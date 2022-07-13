using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using Prism.Mvvm;
using Prism.Commands;
using PairMatching.DomainModel.Services;
using Prism.Events;
using GuiWpf.Events;
using MahApps.Metro.Controls.Dialogs;

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : BindableBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;
        readonly IDialogCoordinator _dialog;

        public ParticipiantsViewModel(IParticipantService participantService, IEventAggregator ea, IDialogCoordinator dialog)
        {
            _participantService = participantService;
            _ea = ea;

            SubscribeToEvents();
            _dialog = dialog;
        }

        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
               async () =>
               {
                   //await MetroProgressOnLoading();
                   if (IsInitialized)
                   {
                       return;
                   }
                   IsLoaded = true;
                   var parts = await _participantService.GetAll();                 
                   _participiants.Clear();
                   _participiants.AddRange(parts);
                   Participiants.Clear();
                   Participiants.AddRange(parts);
                   IsInitialized = true;
                   IsLoaded = false;
               });

        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get => _isInitialized ;
            set => SetProperty(ref _isInitialized , value);
        }

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(ref _isLoaded, value);
        }

        public async Task MetroProgressOnLoading()
        {
            // Show...
            ProgressDialogController controller = await _dialog.ShowProgressAsync(this, "Wait", "Waiting for the Answer to the Ultimate Question of Life, The Universe, and Everything...");

            controller.SetIndeterminate();

            var parts = await _participantService.GetAll();
            Participiants.Clear();
            Participiants.AddRange(parts);

            // Close...
            await controller.CloseAsync();
        }

        private void SubscribeToEvents()
        {
            _ea.GetEvent<CloseDialogEvent>().Subscribe(CloseFormResived);
            _ea.GetEvent<AddParticipantEvent>().Subscribe( async (part) =>
            {
                Participiants.Add(part);
                await _participantService.UpserteParticipant(part);
            });
        }

        public ObservableCollection<Participant> _participiants = new(); 

        public ObservableCollection<Participant> Participiants { get; set; } = new();

        private Participant _selectedParticipant = new();
        public Participant SelectedParticipant
        {
            get => _selectedParticipant; 
            set 
            { 
                if(SetProperty(ref _selectedParticipant, value))
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

        private bool _isToggleRow = false;
        public bool IsToggleRow
        {
            get => _isToggleRow; 
            set => SetProperty(ref _isToggleRow, value); 
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

        DelegateCommand _toggleRow;
        public DelegateCommand ToggleRow => _toggleRow ??= new(
            () =>
            {
                IsToggleRow = !IsToggleRow;
            });

        private string _searchParticipiantsWord = "";
        public string SearchParticipiantsWord
        {
            get { return _searchParticipiantsWord; }
            set 
            { 
                SetProperty(ref _searchParticipiantsWord, value); 
            }
        }

        DelegateCommand _searchParticipiantsCommand;
        public DelegateCommand SearchParticipiantsCommand => _searchParticipiantsCommand ??= new(
             () =>
            {
                if (string.IsNullOrEmpty(SearchParticipiantsWord))
                {
                    Participiants.Clear();
                    Participiants.AddRange(_participiants);
                    return;
                }
                var list = Participiants
                .Where(p => p.Name
                    .Contains(SearchParticipiantsWord, StringComparison.CurrentCultureIgnoreCase));
                if (list.Any())
                {
                    Participiants.Clear();
                    Participiants.AddRange(list);
                }
 
            },
             () => !IsLoaded);
    }
}
