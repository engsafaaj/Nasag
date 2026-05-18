using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nasag.ViewModels.Pages;

public abstract partial class PageViewModel : ObservableObject
{
    public abstract string TitleAr { get; }
    public virtual string SubtitleAr => string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _statusMessage;

    /// <summary>
    /// Called by the shell whenever the page becomes active. Override to load data.
    /// </summary>
    public virtual Task ActivateAsync(CancellationToken ct = default) => Task.CompletedTask;
}

public sealed class FeesViewModel : PageViewModel
{
    public override string TitleAr => "الرسوم والأقساط";
    public override string SubtitleAr => "إدارة رسوم الطلاب والأقساط وتسجيل الدفعات";
}

public sealed class ReportsViewModel : PageViewModel
{
    public override string TitleAr => "مركز التقارير";
    public override string SubtitleAr => "استخراج التقارير المختلفة وطباعتها";
}

public sealed class UsersViewModel : PageViewModel
{
    public override string TitleAr => "المستخدمون";
    public override string SubtitleAr => "إدارة المستخدمين والأدوار والصلاحيات";
}

public sealed partial class SettingsViewModel : PageViewModel
{
    private readonly Nasag.Services.IUserPreferencesService _prefs;
    private readonly Nasag.Services.IToastService _toasts;

    public SettingsViewModel(
        Nasag.Services.IUserPreferencesService prefs,
        Nasag.Services.IToastService toasts)
    {
        _prefs = prefs;
        _toasts = toasts;
        _studentsSortAlphabetically = _prefs.Current.StudentsSortAlphabetically;
    }

    public override string TitleAr => "الإعدادات";
    public override string SubtitleAr => "بيانات المدرسة والسنة الدراسية والإعدادات العامة";

    [ObservableProperty]
    private bool _studentsSortAlphabetically;

    partial void OnStudentsSortAlphabeticallyChanged(bool value)
    {
        _prefs.Current.StudentsSortAlphabetically = value;
        _prefs.Save();
        _toasts.Success("تم حفظ الإعداد",
            value ? "سيتم ترتيب الطلاب أبجدياً." : "سيظهر الطالب الأحدث في الأعلى.");
    }
}

public sealed class BackupViewModel : PageViewModel
{
    public override string TitleAr => "النسخ الاحتياطي";
    public override string SubtitleAr => "إنشاء النسخ الاحتياطية واسترجاعها";
}
