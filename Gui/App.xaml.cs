using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PairMatching.DomainModel.Domains;
using PairMatching.DomainModel.Email;
using PairMatching.DomainModel.GoogleSheet;
using PairMatching.Gui.ViewModels;
using PairMatching.Gui.Views;
using PairMatching.Models;
using System;
using System.IO;
using System.Windows;

namespace PairMatching.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider(); 

            //var viewModel = ServiceProvider.GetRequiredService<MainViewModel>();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Current.Shutdown();
            base.OnExit(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            
            var mailSettings = new MailSettings();
            Configuration.GetSection("MailSettings:Test").Bind(mailSettings);
           
            services.AddTransient(x => mailSettings);
            services.AddTransient<SendEmail>();

            var configDataAccess = new ConfigDataAccess
            {
                ConnctionsStrings = Configuration.GetConnectionString("Remote"),
                DataAccessType = Configuration.GetSection("DataAccess")["Default"]
            };
            services.AddSingleton(x => configDataAccess);

            services.AddSingleton<DomainsContainer>();
            
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        }
    }
}
