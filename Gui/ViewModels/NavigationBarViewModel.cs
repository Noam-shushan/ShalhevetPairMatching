using PairMatching.DomainModel.Domains;
using PairMatching.Gui.Commands;
using System;

namespace PairMatching.Gui.ViewModels
{
    public enum NavigationCurrentView
    {
        Students,
        Pairs,
        ActivePairs,
        NotActivePairs,
        Statistics,
        StudentFromIsrael,
        StudentFormWorld,
        StudentWithoutPair,
        ArchiveStudents
    }

    public class NavigationBarViewModel 
    {
        private MainViewModelBase _selectedViewModel;

        public event Action ViewChanged;
        
        public MainViewModelBase SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                ViewChanged?.Invoke();
            }
        }

        public ChangeMainViewCommand ChangeMainView { get; set; }

        private StudentsListViewModel studentsVM;
        private PairsListViewModel pairsVM;
        private StatisticsViewModel statisticsVM;

        readonly DomainsContainer _domains;

        public NavigationBarViewModel(DomainsContainer domains)
        {
            _domains = domains;
            studentsVM = new StudentsListViewModel();
            pairsVM = new PairsListViewModel();
            statisticsVM = new StatisticsViewModel();

            ChangeMainView = new ChangeMainViewCommand();
            ChangeMainView.ChangeMainView += ChangeMainView_ChangeMainView;
        }

        private void ChangeMainView_ChangeMainView(NavigationCurrentView? view)
        {
            switch (view)
            {
                case NavigationCurrentView.Students:
                    studentsVM.ListFilter = _ => true;
                    SelectedViewModel = studentsVM;
                    break;
                
                case NavigationCurrentView.Pairs:
                    pairsVM.ListFilter = _ => true;
                    SelectedViewModel = pairsVM;
                    break;
                
                case NavigationCurrentView.Statistics:
                    SelectedViewModel = statisticsVM;
                    break;
                
                case NavigationCurrentView.StudentWithoutPair:
                    studentsVM.ListFilter = s => (s as StudentViewModel).IsOpenToMatch;
                    SelectedViewModel = studentsVM;
                    break;
                
                case NavigationCurrentView.StudentFromIsrael:
                    studentsVM.ListFilter = s => (s as StudentViewModel).IsFromIsrael;
                    SelectedViewModel = studentsVM;
                    break;
                
                case NavigationCurrentView.StudentFormWorld:
                    studentsVM.ListFilter = s => !(s as StudentViewModel).IsFromIsrael;
                    SelectedViewModel = studentsVM;
                    break;
                
                case NavigationCurrentView.ArchiveStudents:
                    studentsVM.ListFilter = s => (s as StudentViewModel).IsInArchive;
                    SelectedViewModel = studentsVM;
                    break;
                
                case NavigationCurrentView.ActivePairs:
                    pairsVM.ListFilter = p => (p as PairViewModel).IsActive;
                    SelectedViewModel = pairsVM;
                    break;
                
                case NavigationCurrentView.NotActivePairs:
                    pairsVM.ListFilter = p => !(p as PairViewModel).IsActive;
                    SelectedViewModel = pairsVM;
                    break;
                default:
                    break;
            }
        }
    }
}