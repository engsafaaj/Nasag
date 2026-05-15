using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nasag.Services;
using Nasag.ViewModels.Pages;
using Nasag.ViewModels.Shell;
using Nasag.Views.Shell;

namespace Nasag;

public partial class App : Application
{
    public static IHost? Host { get; private set; }

    public static T GetService<T>() where T : class
        => Host?.Services.GetRequiredService<T>()
           ?? throw new InvalidOperationException("Host is not initialized.");

    protected override void OnStartup(StartupEventArgs e)
    {
        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((_, services) => ConfigureServices(services))
            .Build();

        Host.Start();

        var shell = new MainShellView
        {
            DataContext = GetService<MainShellViewModel>()
        };
        shell.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (Host is not null)
        {
            await Host.StopAsync();
            Host.Dispose();
            Host = null;
        }
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Cross-cutting services
        services.AddSingleton<IAppInfoService, AppInfoService>();
        services.AddSingleton<IBusyService, BusyService>();
        services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
        services.AddSingleton<INavigationService, NavigationService>();

        // Shell
        services.AddSingleton<MainShellViewModel>();

        // Page VMs (singletons so they keep their state during the session)
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<StudentsViewModel>();
        services.AddSingleton<ClassesViewModel>();
        services.AddSingleton<AttendanceViewModel>();
        services.AddSingleton<SubjectsViewModel>();
        services.AddSingleton<MarksViewModel>();
        services.AddSingleton<ResultsViewModel>();
        services.AddSingleton<FeesViewModel>();
        services.AddSingleton<ReportsViewModel>();
        services.AddSingleton<UsersViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<BackupViewModel>();
    }
}
