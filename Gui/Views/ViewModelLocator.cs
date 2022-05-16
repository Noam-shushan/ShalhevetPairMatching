using PairMatching.Gui.ViewModels;

namespace PairMatching.Gui.Views
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel =>
            App.ServiceProvider.GetService(typeof(MainViewModel)) as MainViewModel;

        public NavigationBarViewModel NavigationBarViewModel => MainViewModel.NavigationBar;
    }
}
