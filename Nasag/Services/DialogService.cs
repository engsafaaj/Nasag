using System.Threading.Tasks;
using System.Windows;

namespace Nasag.Services;

public sealed class DialogService : IDialogService
{
    public Task<bool> ConfirmAsync(string title, string message, string okText = "تأكيد", string cancelText = "إلغاء")
    {
        return InvokeAsync(() =>
        {
            var owner = ResolveOwner();
            var result = MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return result == MessageBoxResult.OK;
        });
    }

    public Task ShowInfoAsync(string title, string message)
    {
        return InvokeAsync(() =>
        {
            var owner = ResolveOwner();
            MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.OK,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return true;
        });
    }

    public Task ShowErrorAsync(string title, string message)
    {
        return InvokeAsync(() =>
        {
            var owner = ResolveOwner();
            MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return true;
        });
    }

    private static Window? ResolveOwner()
        => Application.Current?.Windows.Count > 0 ? Application.Current.MainWindow : null;

    private static Task<T> InvokeAsync<T>(System.Func<T> action)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            return Task.FromResult(action());

        var tcs = new TaskCompletionSource<T>();
        dispatcher.BeginInvoke(new System.Action(() =>
        {
            try { tcs.SetResult(action()); }
            catch (System.Exception ex) { tcs.SetException(ex); }
        }));
        return tcs.Task;
    }
}
