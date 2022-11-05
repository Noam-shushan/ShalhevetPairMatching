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

namespace GuiWpf.ViewModels
{
    
    
    public class NotesViewModel : BindableBase
    {
        readonly IEventAggregator _ea;

        readonly ParticipantService _participantService;

        readonly PairService _pairService;
        
        BaseModel _model;

        public NotesViewModel(IEventAggregator ea, ParticipantService participantService, PairService pairService)
        {
            _ea = ea;
            
            _participantService = participantService;
            _pairService = pairService;
            
            //_ea.GetEvent<GetNotesListEvent>()
            //    .Subscribe(NewNotesListResivd);
            _ea.GetEvent<ModelEnterEvent>()
                .Subscribe((model) =>
                {
                    _model = model.Item1;
                    Notes.Clear();
                    Notes.AddRange(_model.Notes);
                    ModelType = model.Item2;
                });
        } 

        private void NewNotesListResivd(IEnumerable<Note> notes)
        {
            Notes.Clear();
            Notes.AddRange(notes);
        }


        private ModelType _modelType;
        public ModelType ModelType
        {
            get => _modelType;
            set => SetProperty(ref _modelType, value);
        }

        public ObservableCollection<Note> Notes { get; set; } = new();

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

        DelegateCommand _AddNoteCommand;
        public DelegateCommand AddNoteCommand => _AddNoteCommand ??= new(
        async () =>
        {
            var newNote = new Note
            {
                Author = Author,
                Content = Content,
                Date = DateTime.Now,
                Subject = Subject
            };
            
            Notes.Add(newNote);
            _model.Notes.Add(newNote);
            switch (ModelType)
            {

                case ModelType.Participant:
                    await _participantService.UpdateParticipaint(_model as Participant);
                    //_ea.GetEvent<NewNoteForParticipaintEvent>().Publish((_modelId, newNote));
                    break;
                case ModelType.Pair:
                    await _pairService.UpdatePair(_model as Pair);
                    //_ea.GetEvent<NewNoteForPairEvent>().Publish((_modelId, newNote));
                    break;
            }
            Reset();
            IsNewNoteFormOpen = !IsNewNoteFormOpen;
        });

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
        });

        private void Reset()
        {
            Subject = Content = string.Empty;
        }
    }
}
