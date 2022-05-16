using PairMatching.DomainModel.Domains;

namespace PairMatching.Gui.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        public NavigationBarViewModel NavigationBar { get; set; }

        public MainViewModelBase CurrentViewModel
        {
            get => NavigationBar.SelectedViewModel;
        }
        
        readonly DomainsContainer _domains;

        public MainViewModel(DomainsContainer domains) 
        {
            _domains = domains;
            NavigationBar = new NavigationBarViewModel(_domains)
            {
                SelectedViewModel = new StudentsListViewModel()
            };

            NavigationBar.ViewChanged += NavigationBar_ViewChanged;
        }

        private void NavigationBar_ViewChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
