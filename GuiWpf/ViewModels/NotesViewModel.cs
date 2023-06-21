using PairMatching.DomainModel.Services;
using PairMatching.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace GuiWpf.ViewModels
{   
    public class NotesViewModel : BindableBase
    {
        readonly IParticipantService _participantService;
        readonly IPairsService _pairService;
        readonly ExceptionHeandler _exceptionHeandler;


        
        public NotesViewModel(IParticipantService participantService, IPairsService pairService, ExceptionHeandler exceptionHeandler) 
        {
            _participantService = participantService;
            _pairService = pairService;
            _exceptionHeandler = exceptionHeandler;
        }

        private BaseModel _currentModel;
        public BaseModel CurrentModel
        {
            get => _currentModel;
            set => SetProperty(ref _currentModel, value);
        }

        public void Init(BaseModel model)
        {
            CurrentModel = model;
            Notes.Clear();
            Notes.AddRange(CurrentModel.Notes);
        }

        public ObservableCollection<Note> Notes { get; set; } = new();

        #region Properties
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
            try
            {
                await AddNote();
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        }, () => true);



        DelegateCommand _OpenNewNoteFormCommand;
        public DelegateCommand OpenNewNoteFormCommand => _OpenNewNoteFormCommand ??= new(
        () =>
        {
            IsNewNoteFormOpen = !IsNewNoteFormOpen;
        });

        DelegateCommand _DeleteNoteCommand;
        public DelegateCommand DeleteNoteCommand => _DeleteNoteCommand ??= new(
        async () =>
        {
            try
            {
                await DeleteNote();
            }
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
        });
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
            
            if (CurrentModel is Participant participant)
            {
                await _participantService.AddNote(newNote, participant);
            }
            else if (CurrentModel is Pair pair)
            {
                await _pairService.AddNote(newNote, pair);
            }
            Reset();
            IsNewNoteFormOpen = !IsNewNoteFormOpen;
        }
        
        private async Task DeleteNote()
        {
            var noteToDelete = SelectedNote;
            Notes.Remove(SelectedNote);
            if (CurrentModel is Participant participant)
            {
                await _participantService.DeleteNote(noteToDelete, participant);
            }
            else if (CurrentModel is Pair pair)
            {
                await _pairService.DeleteNote(noteToDelete, pair);
            }
        }

        private void Reset()
        {
            Subject = Content = string.Empty;
        }
    }
}
