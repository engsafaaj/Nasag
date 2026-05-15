using CommunityToolkit.Mvvm.ComponentModel;
using Nasag.Services;

namespace Nasag.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _appName;

    [ObservableProperty]
    private string _tagline;

    [ObservableProperty]
    private string _version;

    public MainViewModel(IAppInfoService appInfo)
    {
        _appName = appInfo.AppNameAr;
        _tagline = appInfo.AppTagline;
        _version = appInfo.Version;
    }
}
