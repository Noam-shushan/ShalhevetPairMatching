using GuiWpf.Views;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Configurations;
using Newtonsoft.Json;
using PairMatching.DomainModel.Email;
using PairMatching.DomainModel.Services;
using static PairMatching.Tools.HelperFunction;
using Prism.Mvvm;
using GuiWpf.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using Root;

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
            //AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            _startup = new Startup();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(_startup.GetConfigurations());
            containerRegistry.Register<IDataAccessFactory, DataAccessFactory>();

            containerRegistry.Register<SendEmail>();
            containerRegistry.Register<IParticipantService, ParticipantService>();
            containerRegistry.Register<IPairsService, PairService>();
            containerRegistry.Register<IEmailService, EmailService>();
            containerRegistry.Register<IMatchingService, MatchingService>();

            containerRegistry.Register<IDialogCoordinator, DialogCoordinator>();
        }



        protected override Window CreateShell()
        {
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
            ViewModelLocationProvider.Register<NotesView, NotesViewModel>();
            ViewModelLocationProvider.Register<SendEmailView, SendEmailViewModel>();
        }
    }
}
