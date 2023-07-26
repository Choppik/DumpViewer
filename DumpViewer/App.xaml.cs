using DumpViewer.Services;
using DumpViewer.Store;
using DumpViewer.ViewModels;
using DumpViewer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace DumpViewer
{
    public partial class App : Application
    {
        private readonly IHost _host;
        public App()
        {
            #region Зависимости
            _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<DumpViewerViewModel>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton(s => CreateDumpViewerNavigationService(s));

                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainWindowViewModel>()
                });
            }).Build();
            #endregion
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            INavigationService initialNavigationService = _host.Services.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }

        private static INavigationService CreateDumpViewerNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<DumpViewerViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<DumpViewerViewModel>());
        }
    }
}