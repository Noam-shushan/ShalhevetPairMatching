using GuiWpf.Events;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace GuiWpf.ViewModels
{   
    public class NotesViewModel : BindableBase
    {
        readonly IEventAggregator _ea;

        readonly ParticipantService _participantService;
        

        private Participant _participantModel;


        readonly PairService _pairService;

        private Pair _pairModel;

        //static int count = 0;
        
        public NotesViewModel(IEventAggregator ea, ParticipantService participantService)
        {
            _ea = ea;

            _participantService = participantService;

            _ea.GetEvent<ManageNotesForParticipiantEvent>()
                .Subscribe((participant) =>
                {
                    //++count;
                    //MessageBox.Show($"{count}");
                    _participantModel = participant;
                    Notes.Clear();
                    Notes.AddRange(_participantModel.Notes);                   
                });

            _ea.GetEvent<ManageNotesForPairEvent>()
                .Subscribe((pair) =>
                {
                    _pairModel = pair;
                    Notes.Clear();
                    Notes.AddRange(_pairModel.Notes);
                });
        }

        public ObservableCollection<Note> Notes { get; set; } = new();

        #region Properties
        private ModelType _modelType;
        public ModelType ModelType
        {
            get => _modelType;
            set => SetProperty(ref _modelType, value);
        }

        

        private Note _selectedNote;
        public Note SelectedNote
        {
            get => _selectedNote;
            set => SetProperty(ref _selectedNote, value);
        }

        private string _author;
        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        private string _subject;
        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private bool _isNewNoteFormOpen;
        public bool IsNewNoteFormOpen
        {
            get => _isNewNoteFormOpen;
            set => SetProperty(ref _isNewNoteFormOpen, value);
        }
        #endregion

        #region Commands
        DelegateCommand _AddNoteCommand;
        public DelegateCommand AddNoteCommand => _AddNoteCommand ??= new(
        async () =>
        {
            await AddNote();
        }, () => false);



        DelegateCommand _OpenNewNoteFormCommand;
        public DelegateCommand OpenNewNoteFormCommand => _OpenNewNoteFormCommand ??= new(
        () =>
        {
            IsNewNoteFormOpen = !IsNewNoteFormOpen;
        });

        DelegateCommand _DeleteNoteCommand;
        public DelegateCommand DeleteNoteCommand => _DeleteNoteCommand ??= new(
        () =>
        {
            switch (ModelType)
            {
                case ModelType.Participant:
                    _ea.GetEvent<DeleteNoteFromParticipiantEvent>().Publish(("", SelectedNote));
                    break;
                case ModelType.Pair:
                    _ea.GetEvent<DeleteNoteFromPairEvent>().Publish(("", SelectedNote));
                    break;
            }
            Reset();
            Notes.Remove(SelectedNote);
        }, () => false);
        #endregion


        private async Task AddNote()
        {
            var newNote = new Note
            {
                Author = Author,
                Content = Content,
                Date = DateTime.Now,
                Subject = Subject
            };

            Notes.Add(newNote);

            if (_participantModel != null)
            {
                await _participantService.AddNote(newNote, _participantModel);
            }
            else if (_pairModel != null)
            {
                await _pairService.AddNote(newNote, _pairModel);
            }
            Reset();
            IsNewNoteFormOpen = !IsNewNoteFormOpen;
        }

        private void Reset()
        {
            Subject = Content = string.Empty;
        }
    }
}
