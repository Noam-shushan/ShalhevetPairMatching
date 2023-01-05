using GuiWpf.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DomainModel.Services;
using Prism.Mvvm;
using GuiWpf.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using PairMatching.Root;
using GuiWpf.Commands;
using PairMatching.Loggin;
using System.Threading.Tasks;

namespace GuiWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private readonly Startup _startup;

        public App() : base()
        {
            // DispatcherUnhandledException="PrismApplication_DispatcherUnhandledException"
            //AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            _startup = new Startup();
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            var logger = Container.Resolve<Logger>();
            await logger.SendLogs();

            base.OnExit(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.RegisterInstance(_startup.GetConfigurations());

            containerRegistry.RegisterInstance(new Logger(_startup.GetConfigurations().ConnctionsStrings));
            containerRegistry.Register<ExceptionHeandler>();

            containerRegistry.Register<IDataAccessFactory, DataAccessFactory>();

            containerRegistry.RegisterSingleton<MatchCommand>();
            
            containerRegistry.Register<IParticipantService, ParticipantService>();
            containerRegistry.Register<IPairsService, PairService>();
            containerRegistry.Register<IEmailService, EmailService>();
            containerRegistry.RegisterSingleton<IMatchingService, MatchingService>();

            containerRegistry.Register<IDialogCoordinator, DialogCoordinator>();
        }

        protected override Window CreateShell()
        {
            if (!_startup.GetConfigurations().IsTest && !_startup.IsConnectedToInternet())
            {
                MessageBox.Show("Please check your internet connection and try again.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error, MessageBoxResult.None);
                Shutdown();
                return null;
            }
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
            ViewModelLocationProvider.Register<ParticipiantsView, ParticipiantsViewModel>();
            ViewModelLocationProvider.Register<MatchingView, MatchingViewModel>();
            ViewModelLocationProvider.Register<AddParticipantForm, AddParticipantFormViewModel>();
            ViewModelLocationProvider.Register<EmailsView, EmailsViewModel>();
            ViewModelLocationProvider.Register<ParisView, PairsViewModel>();
            ViewModelLocationProvider.Register<SendEmailView, SendEmailViewModel>();
            ViewModelLocationProvider.Register<AutoMatchingView, AutoMatchingViewModel>();
        }
    }
}
