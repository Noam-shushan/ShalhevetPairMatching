using GuiWpf.Events;
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

        public NotesViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<GetNotesListEvent>()
                .Subscribe(NewNotesListResivd);
        }
        private void NewNotesListResivd(IEnumerable<Note> notes)
        {
            Notes.Clear();
            Notes.AddRange(notes);
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
        () =>
        {
            var newNote = new Note
            {
                Author = Author,
                Content = Content,
                Date = DateTime.Now,
                Subject = Subject
            };
            Notes.Add(newNote);
            _ea.GetEvent<NewNoteEvent>().Publish(newNote);
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
            _ea.GetEvent<DeleteNoteEvent>().Publish(SelectedNote);
            Notes.Remove(SelectedNote);
        });

        private void Reset()
        {
            Subject = Content = string.Empty;
        }
    }
}
