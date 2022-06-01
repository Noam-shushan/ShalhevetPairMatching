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

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : BindableBase
    {
        readonly IParticipantService _participantService;
        readonly IEventAggregator _ea;

        public ParticipiantsViewModel(IParticipantService participantService, IEventAggregator ea)
        {
            _participantService = participantService;
            _ea = ea;
            _ea.GetEvent<CloseDialogEvent>().Subscribe(CloseFormResived);
            _ea.GetEvent<AddParticipantEvent>().Subscribe(NewParticipantResived);
        }

        private void NewParticipantResived(Participant part)
        {
            Participiants.Add(part);
        }

        public ObservableCollection<Participant> Participiants { get; set; } = new();

        private Participant _selectedParticipant = new();
        public Participant SelectedParticipant
        {
            get => _selectedParticipant; 
            set 
            { 
                if(SetProperty(ref _selectedParticipant, value))
                {
                    _ea.GetEvent<GetNotesListEvent>()
                        .Publish(SelectedParticipant.Notes);
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


        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
            async () =>
            {
                //var studList = await _participantService.GetAllStudents();
                //var list = studList.Select(s => s.ToParticipant());
                var list = await _participantService.GetParticipantsWix();
                Participiants.Clear();
                Participiants.AddRange(list);
            });

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
            async () =>
            {
                var s = SearchParticipiantsWord;
                await _participantService.UpdateParticipant(SelectedParticipant);
            });
    }
}
