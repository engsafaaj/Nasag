using System.Windows;
using System.Windows.Controls;
using Nasag.ViewModels.Auth;

namespace Nasag.Views.Auth;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        Loaded += (_, _) => UsernameField.Focus();
    }

    /// <summary>
    /// WPF's <see cref="PasswordBox"/> doesn't expose the password as a bindable DP for security reasons;
    /// pipe it to the view model imperatively instead.
    /// </summary>
    private void PasswordField_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            vm.Password = pb.Password;
    }
}
