using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nasag.Services;

namespace Nasag.ViewModels.Auth;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ICurrentUserService _currentUser;

    public LoginViewModel(IAuthService auth, ICurrentUserService currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    public string AppName => "نَسَق لإدارة المدارس";
    public string AppTagline => "كل بيانات المدرسة في نظام واحد بسيط";

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();

            var result = await _auth.SignInAsync(Username.Trim(), Password).ConfigureAwait(true);

            if (result.IsSuccess && result.User is not null)
            {
                _currentUser.SignIn(result.User);
                return;
            }

            ShowError(result.ErrorMessage ?? "تعذّر تسجيل الدخول.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanLogin() => !IsBusy
                               && !string.IsNullOrWhiteSpace(Username)
                               && !string.IsNullOrEmpty(Password);

    partial void OnUsernameChanged(string value)
    {
        ClearError();
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string value)
    {
        ClearError();
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value) => LoginCommand.NotifyCanExecuteChanged();

    private void ClearError()
    {
        HasError = false;
        ErrorMessage = null;
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }
}
