using System.Threading.Tasks;

namespace Nasag.Services;

public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message, string okText = "تأكيد", string cancelText = "إلغاء");
    Task ShowInfoAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
}
