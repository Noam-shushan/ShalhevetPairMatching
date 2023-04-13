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
    class FullParticipaintViewModel : DialogViewModel
    {
        readonly IEventAggregator _ea;
        
        public FullParticipaintViewModel(IEventAggregator ea) : base(ea)
        {
            _ea = ea;
            _ea.GetEvent<OnParticipaintSelected>().Subscribe(
                (p) => 
                {
                    Current = p;
                    LearningTimes = CollectionViewSource.GetDefaultView(Current.PairPreferences.LearningTime);
                    LearningTimes.Filter = l => (l as LearningTime).TimeInDay.Any();
                });
            
        }

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

    }
}
