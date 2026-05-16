using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Nasag.ViewModels;
using Nasag.ViewModels.Pages;
using Nasag.ViewModels.Pages.Attendance;
using Nasag.ViewModels.Pages.Classes;
using Nasag.ViewModels.Pages.Students;

namespace Nasag.Services;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private NavigationSection _current = NavigationSection.Dashboard;
    private object? _currentVm;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
        Descriptors = BuildDescriptors();
    }

    public NavigationSection Current => _current;
    public object? CurrentViewModel => _currentVm;
    public event EventHandler? CurrentChanged;

    public IReadOnlyList<NavigationDescriptor> Descriptors { get; }

    public void NavigateTo(NavigationSection section)
    {
        _current = section;
        _currentVm = Resolve(section);
        CurrentChanged?.Invoke(this, EventArgs.Empty);
    }

    private object Resolve(NavigationSection section) => section switch
    {
        NavigationSection.Dashboard => _services.GetRequiredService<DashboardViewModel>(),
        NavigationSection.Students => _services.GetRequiredService<StudentsViewModel>(),
        NavigationSection.Classes => _services.GetRequiredService<ClassesViewModel>(),
        NavigationSection.Attendance => _services.GetRequiredService<AttendanceViewModel>(),
        NavigationSection.Subjects => _services.GetRequiredService<SubjectsViewModel>(),
        NavigationSection.Marks => _services.GetRequiredService<MarksViewModel>(),
        NavigationSection.Results => _services.GetRequiredService<ResultsViewModel>(),
        NavigationSection.Fees => _services.GetRequiredService<FeesViewModel>(),
        NavigationSection.Reports => _services.GetRequiredService<ReportsViewModel>(),
        NavigationSection.Users => _services.GetRequiredService<UsersViewModel>(),
        NavigationSection.Settings => _services.GetRequiredService<SettingsViewModel>(),
        NavigationSection.Backup => _services.GetRequiredService<BackupViewModel>(),
        _ => throw new InvalidOperationException($"Unknown section {section}")
    };

    private static IReadOnlyList<NavigationDescriptor> BuildDescriptors() => new[]
    {
        new NavigationDescriptor(NavigationSection.Dashboard, "الرئيسية", "IconDashboard"),
        new NavigationDescriptor(NavigationSection.Students, "الطلاب", "IconStudents"),
        new NavigationDescriptor(NavigationSection.Classes, "الصفوف والشعب", "IconClasses"),
        new NavigationDescriptor(NavigationSection.Attendance, "الحضور والغياب", "IconAttendance"),
        new NavigationDescriptor(NavigationSection.Subjects, "المواد والدرجات", "IconSubjects"),
        new NavigationDescriptor(NavigationSection.Marks, "إدخال الدرجات", "IconSubjects"),
        new NavigationDescriptor(NavigationSection.Results, "النتائج", "IconResults"),
        new NavigationDescriptor(NavigationSection.Fees, "الرسوم والأقساط", "IconFees"),
        new NavigationDescriptor(NavigationSection.Reports, "التقارير", "IconReports"),
        new NavigationDescriptor(NavigationSection.Users, "المستخدمون", "IconUsers"),
        new NavigationDescriptor(NavigationSection.Settings, "الإعدادات", "IconSettings"),
        new NavigationDescriptor(NavigationSection.Backup, "النسخ الاحتياطي", "IconBackup"),
    };
}
