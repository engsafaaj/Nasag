using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nasag.Models;
using Nasag.Repositories;
using Nasag.Services;

namespace Nasag.ViewModels.Pages.Students;

public sealed partial class StudentsViewModel : PageViewModel
{
    private readonly IStudentsRepository _repo;
    private readonly IDialogService _dialogs;
    private readonly StudentEditorViewModel _editor;

    private List<SectionOption> _allSections = new();
    private CancellationTokenSource? _searchCts;

    public StudentsViewModel(
        IStudentsRepository repo,
        IDialogService dialogs,
        StudentEditorViewModel editor)
    {
        _repo = repo;
        _dialogs = dialogs;
        _editor = editor;
        _editor.Saved += OnEditorSaved;
        _editor.Cancelled += OnEditorCancelled;

        StatusOptions = new[]
        {
            new StudentStatusFilter(null, "جميع الحالات"),
            new StudentStatusFilter(StudentStatus.Active, "نشط"),
            new StudentStatusFilter(StudentStatus.Archived, "مؤرشف"),
            new StudentStatusFilter(StudentStatus.Graduated, "متخرّج"),
        };
        SelectedStatus = StatusOptions[0];

        PageSizeOptions = new[] { 10, 20, 50, 100 };
        PageSize = 20;
    }

    public override string TitleAr => "الطلاب";
    public override string SubtitleAr => CurrentMode == StudentsPageMode.Editor
        ? "أدخل بيانات الطالب وولي الأمر ثم احفظ."
        : "إدارة بيانات الطلاب والبحث والفلترة";

    public StudentEditorViewModel Editor => _editor;

    public ObservableCollection<StudentRow> Students { get; } = new();
    public ObservableCollection<GradeOption> Grades { get; } = new();
    public ObservableCollection<SectionOption> AvailableSections { get; } = new();
    public IReadOnlyList<StudentStatusFilter> StatusOptions { get; }
    public IReadOnlyList<int> PageSizeOptions { get; }

    [ObservableProperty] private StudentsPageMode _currentMode = StudentsPageMode.List;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private GradeOption? _selectedGrade;
    [ObservableProperty] private SectionOption? _selectedSection;
    [ObservableProperty] private StudentStatusFilter _selectedStatus;
    [ObservableProperty] private int _page = 1;
    [ObservableProperty] private int _pageSize;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _activeCount;
    [ObservableProperty] private int _archivedCount;
    [ObservableProperty] private int _allCount;

    public bool ShowList => CurrentMode == StudentsPageMode.List;
    public bool ShowEditor => CurrentMode == StudentsPageMode.Editor;
    public bool HasResults => Students.Count > 0;
    public bool IsEmpty => !IsLoading && Students.Count == 0;
    public bool CanGoNext => Page < TotalPages;
    public bool CanGoPrev => Page > 1;

    public string PaginationLabel => TotalCount == 0
        ? "لا توجد نتائج"
        : $"الصفحة {Page} من {Math.Max(TotalPages, 1)} — إجمالي {TotalCount:N0}";

    partial void OnCurrentModeChanged(StudentsPageMode value)
    {
        OnPropertyChanged(nameof(ShowList));
        OnPropertyChanged(nameof(ShowEditor));
        OnPropertyChanged(nameof(SubtitleAr));
    }

    partial void OnPageChanged(int value)
    {
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrev));
        OnPropertyChanged(nameof(PaginationLabel));
        NextPageCommand.NotifyCanExecuteChanged();
        PrevPageCommand.NotifyCanExecuteChanged();
    }

    partial void OnTotalPagesChanged(int value)
    {
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrev));
        OnPropertyChanged(nameof(PaginationLabel));
        NextPageCommand.NotifyCanExecuteChanged();
        PrevPageCommand.NotifyCanExecuteChanged();
    }

    partial void OnTotalCountChanged(int value) => OnPropertyChanged(nameof(PaginationLabel));

    partial void OnSearchTextChanged(string value) => DebouncedReload();
    partial void OnSelectedStatusChanged(StudentStatusFilter value) => ResetPageAndReload();
    partial void OnPageSizeChanged(int value) => ResetPageAndReload();

    partial void OnSelectedGradeChanged(GradeOption? value)
    {
        AvailableSections.Clear();
        if (value is not null)
        {
            foreach (var s in _allSections.Where(s => s.GradeId == value.Id))
                AvailableSections.Add(s);
        }
        if (SelectedSection is not null && SelectedSection.GradeId != value?.Id)
            SelectedSection = null;
        ResetPageAndReload();
    }

    partial void OnSelectedSectionChanged(SectionOption? value) => ResetPageAndReload();

    public override async Task ActivateAsync(CancellationToken ct = default)
    {
        if (Grades.Count == 0)
            await LoadLookupsAsync(ct).ConfigureAwait(true);
        await ReloadAsync(ct).ConfigureAwait(true);
    }

    private async Task LoadLookupsAsync(CancellationToken ct)
    {
        try
        {
            var lookups = await _repo.GetLookupsAsync(ct).ConfigureAwait(true);
            Grades.Clear();
            foreach (var g in lookups.Grades) Grades.Add(g);
            _allSections = lookups.Sections.ToList();
        }
        catch (Exception ex)
        {
            StatusMessage = "تعذّر تحميل الصفوف والشعب: " + ex.Message;
        }
    }

    [RelayCommand]
    public async Task ReloadAsync(CancellationToken ct = default)
    {
        try
        {
            IsLoading = true;
            StatusMessage = null;

            var stats = await _repo.GetStatsAsync(ct).ConfigureAwait(true);
            AllCount = stats.Total;
            ActiveCount = stats.Active;
            ArchivedCount = stats.Archived;

            var query = new StudentsQuery(
                Search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                GradeId: SelectedGrade?.Id,
                SectionId: SelectedSection?.Id,
                Status: SelectedStatus?.Value,
                Page: Page,
                PageSize: PageSize);

            var page = await _repo.SearchAsync(query, ct).ConfigureAwait(true);

            Students.Clear();
            foreach (var row in page.Items) Students.Add(row);
            TotalCount = page.TotalCount;
            TotalPages = page.TotalPages;
            if (Page > TotalPages && TotalPages > 0)
            {
                Page = TotalPages;
                await ReloadAsync(ct).ConfigureAwait(true);
                return;
            }
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch (Exception ex)
        {
            StatusMessage = "تعذّر تحميل قائمة الطلاب: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ResetPageAndReload()
    {
        Page = 1;
        _ = ReloadAsync();
    }

    private void DebouncedReload()
    {
        _searchCts?.Cancel();
        var cts = new CancellationTokenSource();
        _searchCts = cts;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, cts.Token).ConfigureAwait(false);
                if (cts.IsCancellationRequested) return;
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Page = 1;
                    return ReloadAsync(cts.Token);
                });
            }
            catch (TaskCanceledException) { /* expected */ }
        });
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SelectedGrade = null;
        SelectedSection = null;
        SelectedStatus = StatusOptions[0];
        SearchText = string.Empty;
        Page = 1;
        _ = ReloadAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        if (!CanGoNext) return;
        Page++;
        await ReloadAsync().ConfigureAwait(true);
    }

    [RelayCommand(CanExecute = nameof(CanGoPrev))]
    private async Task PrevPageAsync()
    {
        if (!CanGoPrev) return;
        Page--;
        await ReloadAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task AddStudentAsync()
    {
        await _editor.LoadForCreateAsync().ConfigureAwait(true);
        CurrentMode = StudentsPageMode.Editor;
    }

    [RelayCommand]
    private async Task EditStudentAsync(StudentRow? row)
    {
        if (row is null) return;
        await _editor.LoadForEditAsync(row.Id).ConfigureAwait(true);
        CurrentMode = StudentsPageMode.Editor;
    }

    [RelayCommand]
    private async Task ArchiveStudentAsync(StudentRow? row)
    {
        if (row is null) return;
        var ok = await _dialogs.ConfirmAsync(
            "أرشفة الطالب",
            $"هل تريد أرشفة الطالب «{row.FullName}»؟ لن يظهر في القائمة الافتراضية.").ConfigureAwait(true);
        if (!ok) return;

        try
        {
            await _repo.SetStatusAsync(row.Id, StudentStatus.Archived).ConfigureAwait(true);
            await ReloadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync("تعذّر الأرشفة", ex.Message).ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task RestoreStudentAsync(StudentRow? row)
    {
        if (row is null) return;
        try
        {
            await _repo.SetStatusAsync(row.Id, StudentStatus.Active).ConfigureAwait(true);
            await ReloadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync("تعذّر استعادة الطالب", ex.Message).ConfigureAwait(true);
        }
    }

    private async void OnEditorSaved(object? sender, EventArgs e)
    {
        CurrentMode = StudentsPageMode.List;
        await ReloadAsync().ConfigureAwait(true);
    }

    private void OnEditorCancelled(object? sender, EventArgs e)
        => CurrentMode = StudentsPageMode.List;
}

public enum StudentsPageMode { List, Editor }

public sealed record StudentStatusFilter(StudentStatus? Value, string Label);
