using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PairMatching.DomainModel.Domains;
using PairMatching.DomainModel.Email;
using PairMatching.DomainModel.GoogleSheet;
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
        public IServiceProvider ServiceProvider { get; private set; }

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

            services.AddTransient<IStudentDescriptor, HebrewDescriptor>();
            services.AddTransient<IStudentDescriptor, EnglishDiscriptor>();

            services.AddSingleton<DomainsContainer>();
            services.AddTransient<MainWindow>();
        }
    }
}
