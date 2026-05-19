using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nasag.Data;
using Nasag.Repositories;
using Nasag.Services;
using Nasag.ViewModels.Auth;
using Nasag.ViewModels.Pages;
using Nasag.ViewModels.Pages.Attendance;
using Nasag.ViewModels.Pages.Classes;
using Nasag.ViewModels.Pages.Exams;
using Nasag.ViewModels.Pages.Fees;
using Nasag.ViewModels.Pages.Marks;
using Nasag.ViewModels.Pages.Results;
using Nasag.ViewModels.Pages.Students;
using Nasag.ViewModels.Pages.Subjects;
using Nasag.ViewModels.Shell;
using Nasag.Views.Auth;
using Nasag.Views.Shell;

namespace Nasag;

public partial class App : Application
{
    public static IHost? Host { get; private set; }

    private LoginView? _loginWindow;
    private MainShellView? _shellWindow;

    public static T GetService<T>() where T : class
        => Host?.Services.GetRequiredService<T>()
           ?? throw new InvalidOperationException("Host is not initialized.");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((ctx, services) => ConfigureServices(services, ctx.Configuration))
            .Build();

        await Host.StartAsync();

        // Global exception handlers — funnel everything through ErrorReporter.
        var reporter = GetService<IErrorReporter>();
        DispatcherUnhandledException += (_, args) =>
        {
            reporter.Report("خطأ غير متوقع", args.Exception.Message, args.Exception);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                reporter.Report("خطأ غير متوقع", ex.Message, ex);
        };
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            reporter.Report("خطأ في مهمة خلفية", args.Exception.Message, args.Exception);
            args.SetObserved();
        };

        // 1) Initialize database (apply pending migrations + seed if empty).
        var initializer = GetService<IDatabaseInitializer>();
        var result = await Task.Run(() => initializer.InitializeAsync()).ConfigureAwait(true);

        if (!result.IsSuccess)
        {
            var details = string.IsNullOrWhiteSpace(result.Details) ? string.Empty : $"\n\nالتفاصيل: {result.Details}";
            Nasag.Views.Common.NasaqDialog.Show(
                null,
                "نَسَق — تعذّر بدء التشغيل",
                $"{result.ErrorMessage}{details}",
                Nasag.Views.Common.NasaqDialogKind.Danger);
            Shutdown(-1);
            return;
        }

        // 2) Wire auth lifecycle: Login -> Shell -> Login on logout.
        var currentUser = GetService<ICurrentUserService>();
        currentUser.SignedIn += OnSignedIn;
        currentUser.SignedOut += OnSignedOut;

        ShowLoginWindow();
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

    private void ShowLoginWindow()
    {
        _loginWindow = new LoginView
        {
            DataContext = GetService<LoginViewModel>()
        };
        _loginWindow.Closed += OnLoginClosed;
        MainWindow = _loginWindow;
        _loginWindow.Show();
    }

    private void OnSignedIn(object? sender, EventArgs e)
    {
        // Spin up the shell, swap MainWindow, then close the login window.
        _shellWindow = new MainShellView
        {
            DataContext = GetService<MainShellViewModel>()
        };
        _shellWindow.Closed += OnShellClosed;
        MainWindow = _shellWindow;
        _shellWindow.Show();

        if (_loginWindow is not null)
        {
            _loginWindow.Closed -= OnLoginClosed;
            _loginWindow.Close();
            _loginWindow = null;
        }
    }

    private void OnSignedOut(object? sender, EventArgs e)
    {
        ShowLoginWindow();

        if (_shellWindow is not null)
        {
            _shellWindow.Closed -= OnShellClosed;
            _shellWindow.Close();
            _shellWindow = null;
        }
    }

    private void OnLoginClosed(object? sender, EventArgs e)
    {
        // User dismissed the login window without signing in -> exit the app.
        if (Current is not null) Current.Shutdown(0);
    }

    private void OnShellClosed(object? sender, EventArgs e)
    {
        // Closing the shell directly (X button) ends the session.
        if (Current is not null) Current.Shutdown(0);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // EF Core — pooled factory so any service can grab a short-lived context.
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection in appsettings.json");

        services.AddDbContextFactory<NasaqDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(NasaqDbContext).Assembly.GetName().Name);
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        });

        // Data services
        services.AddSingleton<IDbSeeder, DbSeeder>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
        services.AddSingleton<IStudentsRepository, StudentsRepository>();
        services.AddSingleton<IClassesRepository, ClassesRepository>();
        services.AddSingleton<IAttendanceRepository, AttendanceRepository>();
        services.AddSingleton<ISubjectsRepository, SubjectsRepository>();
        services.AddSingleton<IExamsRepository, ExamsRepository>();
        services.AddSingleton<IMarksRepository, MarksRepository>();
        services.AddSingleton<IResultsRepository, ResultsRepository>();
        services.AddSingleton<IFeesRepository, FeesRepository>();
        services.AddSingleton<IResultsCalculator, ResultsCalculator>();

        // Cross-cutting services
        services.AddSingleton<IAppInfoService, AppInfoService>();
        services.AddSingleton<IBusyService, BusyService>();
        services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IErrorReporter, ErrorReporter>();
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
        services.AddSingleton<IExcelService, ExcelService>();

        // Auth
        services.AddTransient<LoginViewModel>();

        // Shell
        services.AddSingleton<MainShellViewModel>();

        // Page VMs (singletons so they keep their state during the session)
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<StudentEditorViewModel>();
        services.AddTransient<StudentImportWizardViewModel>();
        services.AddSingleton<StudentsViewModel>();
        services.AddSingleton<ClassesViewModel>();
        services.AddSingleton<AttendanceViewModel>();
        services.AddSingleton<SubjectsViewModel>();
        services.AddSingleton<ExamsViewModel>();
        services.AddSingleton<MarksViewModel>();
        services.AddSingleton<ResultsViewModel>();
        services.AddSingleton<FeesViewModel>();
        services.AddSingleton<ReportsViewModel>();
        services.AddSingleton<UsersViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<BackupViewModel>();
    }
}
