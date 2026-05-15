using System;
using System.Windows.Media;

namespace Nasag.Services;

public enum NavigationSection
{
    Dashboard,
    Students,
    Classes,
    Attendance,
    Subjects,
    Marks,
    Results,
    Fees,
    Reports,
    Users,
    Settings,
    Backup
}

public sealed class NavigationDescriptor
{
    public NavigationSection Section { get; }
    public string TitleAr { get; }
    public string IconKey { get; }

    public NavigationDescriptor(NavigationSection section, string titleAr, string iconKey)
    {
        Section = section;
        TitleAr = titleAr;
        IconKey = iconKey;
    }
}

public interface INavigationService
{
    NavigationSection Current { get; }
    object? CurrentViewModel { get; }
    event EventHandler? CurrentChanged;

    IReadOnlyList<NavigationDescriptor> Descriptors { get; }

    void NavigateTo(NavigationSection section);
}
