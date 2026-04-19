using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JobRunner.Avalonia.ViewModels;
using JobRunner.Avalonia.Views;
using JobRunner.Core.Interfaces;
using JobRunner.Jobs.Extensions;
using JobRunner.WindowsService.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace JobRunner.Avalonia
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            var services = new ServiceCollection();

            services.AddWindowsTaskScheduler();
           // services.AddJobs();  

            services.AddSingleton<MainWindowVM>();

            _serviceProvider = services.BuildServiceProvider();

            var scheduler = _serviceProvider.GetRequiredService<IJobScheduler>();
            scheduler.StartProgramAsync().Wait();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainViewModel = _serviceProvider.GetRequiredService<MainWindowVM>();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}