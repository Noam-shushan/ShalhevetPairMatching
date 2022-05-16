using PairMatching.Models;
using System;

namespace PairMatching.Gui.ViewModels
{
    public class PairViewModel : ViewModelBase
    {
        private Pair _pair;

        public PairViewModel(Pair pair)
        {
            _pair = pair;
            Notes = new NotesViewModel(_pair.Notes);
        }

        public NotesViewModel Notes { get; set; }

        public int Id { get => _pair.Id; }

        /// <summary>
        /// The student from israel 
        /// </summary>
        public Student StudentFromIsrael { get => _pair.StudentFromIsrael; }

        /// <summary>
        /// The macher student from world 
        /// </summary>
        public Student StudentFromWorld { get => _pair.StudentFromWorld; }

        public DateTime DateOfCreate { get => _pair.DateOfCreate; }

        public DateTime DateOfUpdate
        {
            get => _pair.DateOfUpdate;
            set
            {
                _pair.DateOfUpdate = value;
                OnPropertyChanged(nameof(DateOfUpdate));
            }
        }

        public DateTime DateOfDelete
        {
            get => _pair.DateOfDelete;
            set
            {
                _pair.DateOfDelete = value;
                OnPropertyChanged(nameof(DateOfDelete));
            }
        }

        public bool IsActive
        {
            get => _pair.IsActive;
            set
            {
                _pair.IsActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        public string PrefferdTracks
        {
            get => Dictionaries.PrefferdTracksDict[_pair.PrefferdTracks];
            set
            {
                _pair.PrefferdTracks = Dictionaries.PrefferdTracksDictInverse[value];
                OnPropertyChanged(nameof(PrefferdTracks));
            }
        }
    }
}