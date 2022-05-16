using System;

namespace PairMatching.Gui.ViewModels
{
    public abstract class MainViewModelBase : NotifyPropertyChanged
    {
        protected static Predicate<ViewModelBase> BaseFilter { get; } = _ => true;

        Predicate<ViewModelBase> _listFilter = BaseFilter;
        public Predicate<ViewModelBase> ListFilter
        {
            get => _listFilter;
            set
            {
                _listFilter = value;
                OnPropertyChanged(nameof(ListFilter));
            }
        }

        public abstract void Search(string v);

        public SearchViewModel SearchViewModel { get; set; }
    }
}
