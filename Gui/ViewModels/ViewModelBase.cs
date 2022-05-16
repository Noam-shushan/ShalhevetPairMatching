using System;

namespace PairMatching.Gui.ViewModels
{ 
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        public bool IsChanged { get; private set; }

        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        protected override void OnPropertyChanged(string propName)
        {
            base.OnPropertyChanged(propName);
            IsChanged = true;
        }
    }
}
