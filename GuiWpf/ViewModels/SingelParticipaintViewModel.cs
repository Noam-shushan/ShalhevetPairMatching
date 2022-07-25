using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using Prism.Events;
using GuiWpf.Events;

namespace GuiWpf.ViewModels
{
    public class SingelParticipaintViewModel : BindableBase
    {
        readonly IEventAggregator _ea;

        public SingelParticipaintViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<ParticipantEnterEvent>().Subscribe((participant) =>
            {
                ParticipantModel = participant;
            });
        }

        private Participant _participantModel;
        public Participant ParticipantModel
        {
            get => _participantModel;
            set => SetProperty(ref _participantModel, value);
        }

        
    }

    public class SelectableItem<T> : BindableBase
    {

        private string _isSelected;
        public string IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }      
        
        public T Item { get; set; }
    }
}
