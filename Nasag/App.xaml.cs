using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nasag.Services;
using Nasag.ViewModels;

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

        var mainWindow = new MainWindow
        {
            DataContext = GetService<MainViewModel>()
        };
        mainWindow.Show();

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
        services.AddSingleton<IAppInfoService, AppInfoService>();

        services.AddTransient<MainViewModel>();
    }
}
