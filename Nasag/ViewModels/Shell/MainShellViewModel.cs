using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nasag.Services;
using Nasag.ViewModels.Pages;

namespace Nasag.ViewModels.Shell;

public partial class MainShellViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IConnectionMonitor _connection;
    private readonly IAppInfoService _appInfo;
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty]
    private object? _currentPage;

    [ObservableProperty]
    private NavigationItem? _activeItem;

    [ObservableProperty]
    private bool _isDisconnected;

    [ObservableProperty]
    private bool _isSidebarCollapsed;

    [ObservableProperty]
    private string _connectionErrorMessage = "تعذّر الاتصال بقاعدة البيانات. يرجى التحقق من الخادم.";

    [ObservableProperty]
    private string _schoolName = "مدرسة النور الأهلية";

    [ObservableProperty]
    private string _academicYear = "2025 - 2026";

    public string AppName => _appInfo.AppNameAr;
    public string AppTagline => _appInfo.AppTagline;
    public double SidebarWidth => IsSidebarCollapsed ? 78 : 260;
    public string SidebarToggleTooltip => IsSidebarCollapsed ? "توسيع القائمة" : "طي القائمة";

    public string UserDisplayName => _currentUser.DisplayName;
    public string UserInitial => _currentUser.Initial;
    public string? UserRoleName => _currentUser.User?.Role?.NameAr;

    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

    public MainShellViewModel(
        INavigationService navigation,
        IConnectionMonitor connection,
        IAppInfoService appInfo,
        ICurrentUserService currentUser)
    {
        _navigation = navigation;
        _connection = connection;
        _appInfo = appInfo;
        _currentUser = currentUser;

        foreach (var d in _navigation.Descriptors)
            NavigationItems.Add(new NavigationItem(d, NavigateCommand));

        _navigation.CurrentChanged += (_, _) => RefreshFromNavigation();
        _connection.StateChanged += (_, _) => SyncConnectionState();
        _currentUser.SignedIn += (_, _) => OnUserChanged();
        _currentUser.SignedOut += (_, _) => OnUserChanged();
        SyncConnectionState();

        _navigation.NavigateTo(NavigationSection.Dashboard);
    }

    [RelayCommand]
    private void Navigate(NavigationItem? item)
    {
        if (item is null) return;
        _navigation.NavigateTo(item.Section);
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
    }

    [RelayCommand]
    private async Task RetryConnectionAsync()
    {
        var ok = await _connection.CheckAsync();
        if (ok)
            _connection.ReportSuccess();
        else
            _connection.ReportFailure(_connection.LastErrorMessage ?? "تعذّر الاتصال بقاعدة البيانات.");
    }

    [RelayCommand]
    private void Logout()
    {
        _currentUser.SignOut();
    }

    partial void OnIsSidebarCollapsedChanged(bool value)
    {
        OnPropertyChanged(nameof(SidebarWidth));
        OnPropertyChanged(nameof(SidebarToggleTooltip));
    }

    private void OnUserChanged()
    {
        OnPropertyChanged(nameof(UserDisplayName));
        OnPropertyChanged(nameof(UserInitial));
        OnPropertyChanged(nameof(UserRoleName));

        // Reset to Dashboard when a new session begins so the shell never reopens deep-linked.
        if (_currentUser.IsAuthenticated)
            _navigation.NavigateTo(NavigationSection.Dashboard);
    }

    private void RefreshFromNavigation()
    {
        CurrentPage = _navigation.CurrentViewModel;
        foreach (var item in NavigationItems)
            item.IsActive = item.Section == _navigation.Current;
        ActiveItem = NavigationItems.FirstOrDefault(x => x.IsActive);

        if (CurrentPage is PageViewModel page)
            _ = page.ActivateAsync();
    }

    private void SyncConnectionState()
    {
        IsDisconnected = !_connection.IsConnected;
        if (_connection.LastErrorMessage is { Length: > 0 } msg)
            ConnectionErrorMessage = msg;
    }
}

public partial class NavigationItem : ObservableObject
{
    public NavigationSection Section { get; }
    public string TitleAr { get; }
    public string IconKey { get; }
    public CommunityToolkit.Mvvm.Input.IRelayCommand<NavigationItem> Command { get; }

    [ObservableProperty]
    private bool _isActive;

    public NavigationItem(NavigationDescriptor descriptor, CommunityToolkit.Mvvm.Input.IRelayCommand<NavigationItem> command)
    {
        Section = descriptor.Section;
        TitleAr = descriptor.TitleAr;
        IconKey = descriptor.IconKey;
        Command = command;
    }
}
