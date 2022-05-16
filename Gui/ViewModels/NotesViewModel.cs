using PairMatching.DomainModel.Domains;
using PairMatching.Gui.Commands;
using PairMatching.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PairMatching.Gui.ViewModels
{
    public class NotesViewModel : ViewModelBase
    {
        public ObservableCollection<Note> Notes { get; set; }

        private bool _isStudent;
        /// <summary>
        /// Determines if the note is for a student or a pair
        /// </summary>
        public bool IsStudent
        {
            get => _isStudent;
            set
            {
                _isStudent = value;
                OnPropertyChanged(nameof(IsStudent));
            }
        }

        public RelayCommand AddNote { get; set; }

        readonly NotesDomain _notesDomain;


        public NotesViewModel(List<Note> notes)
        {
            var domain = App.ServiceProvider
                                    .GetService(typeof(DomainsContainer)) as DomainsContainer;
            _notesDomain = domain.NotesDomain;

            Notes = new ObservableCollection<Note>(notes);
            Notes.CollectionChanged += Notes_CollectionChanged;

            AddNote = new RelayCommand(
                noteStudentPairWrapper =>
                {
                    if (noteStudentPairWrapper != null)
                    {
                        var note = (noteStudentPairWrapper as dynamic).Note as Note;
                        if (note is not null)
                        {
                            Notes.Add(note);
                        }
                    }

                });
        }

        private void Notes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var noteStudentPairWrapper = e.NewItems[0] as dynamic;
                if (noteStudentPairWrapper != null)
                {
                    var note = noteStudentPairWrapper.Note as Note;
                    var studentOrPair = noteStudentPairWrapper.StudentOrPair;
                    _notesDomain.AddNote(note, studentOrPair);
                }

            }
        }
    }
}