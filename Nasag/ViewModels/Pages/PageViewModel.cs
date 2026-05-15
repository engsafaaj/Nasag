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

public sealed class ClassesViewModel : PageViewModel
{
    public override string TitleAr => "الصفوف والشعب";
    public override string SubtitleAr => "إدارة الصفوف والشعب وتوزيع الطلاب";
}

public sealed class AttendanceViewModel : PageViewModel
{
    public override string TitleAr => "الحضور والغياب";
    public override string SubtitleAr => "تسجيل الحضور اليومي للشعب";
}

public sealed class SubjectsViewModel : PageViewModel
{
    public override string TitleAr => "المواد والامتحانات";
    public override string SubtitleAr => "إدارة المواد الدراسية وأنواع الامتحانات";
}

public sealed class MarksViewModel : PageViewModel
{
    public override string TitleAr => "إدخال الدرجات";
    public override string SubtitleAr => "إدخال درجات الطلاب لكل مادة وامتحان";
}

public sealed class ResultsViewModel : PageViewModel
{
    public override string TitleAr => "نتائج الطلاب";
    public override string SubtitleAr => "عرض المعدلات والتقديرات ونتائج الطلاب";
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

public sealed class SettingsViewModel : PageViewModel
{
    public override string TitleAr => "الإعدادات";
    public override string SubtitleAr => "بيانات المدرسة والسنة الدراسية والإعدادات العامة";
}

public sealed class BackupViewModel : PageViewModel
{
    public override string TitleAr => "النسخ الاحتياطي";
    public override string SubtitleAr => "إنشاء النسخ الاحتياطية واسترجاعها";
}
