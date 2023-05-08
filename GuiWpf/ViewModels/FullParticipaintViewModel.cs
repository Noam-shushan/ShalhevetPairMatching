using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using GuiWpf.Events;
using System.Collections.ObjectModel;
using MongoDB.Driver;
using System.Windows.Data;
using System.ComponentModel;
using Prism.Commands;

namespace GuiWpf.ViewModels
{
    public class FullParticipaintViewModel : ViewModelBase, IPopupViewModle
    {
        public FullParticipaintViewModel()
        {
            ExitPopupVM = new ExitPopupModelViewModel(this);
        }

        public ExitPopupModelViewModel ExitPopupVM { get; set; }

        private Participant _current;
        public Participant Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        private ICollectionView _propName;
        public ICollectionView LearningTimes
        {
            get => _propName;
            set => SetProperty(ref _propName, value);
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        public void Init(Participant participant, bool isOpen)
        {
            Current = participant;
            LearningTimes = CollectionViewSource.GetDefaultView(Current.PairPreferences.LearningTime);
            LearningTimes.Filter = l => (l as LearningTime).TimeInDay.Any();
            IsOpen = isOpen;
        }

        public void CloseDialog()
        {
            IsOpen = false;
        }
    }
}
