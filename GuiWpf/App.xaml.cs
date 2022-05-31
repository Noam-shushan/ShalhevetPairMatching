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

namespace GuiWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(GetConfigurations());
            containerRegistry.Register<IDataAccessFactory, DataAccessFactory>();
            containerRegistry.Register<IParticipantService, ParticipantService>();
            containerRegistry.RegisterScoped<SendEmail>();
        }

        private MyConfiguration GetConfigurations()
        {
            var jsonString = ReadJson(@"appsetting.json");
            var configurations = JsonConvert.DeserializeObject<MyConfiguration>(jsonString);
            return configurations ?? throw new Exception("No Configurations");
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
    }
}
