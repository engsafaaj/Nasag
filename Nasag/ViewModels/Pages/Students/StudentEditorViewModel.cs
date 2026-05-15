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

public sealed partial class StudentEditorViewModel : ObservableObject
{
    private readonly IStudentsRepository _repo;
    private readonly IFileService _files;
    private readonly IDialogService _dialogs;

    private int? _editingId;
    private List<SectionOption> _allSections = new();

    public StudentEditorViewModel(IStudentsRepository repo, IFileService files, IDialogService dialogs)
    {
        _repo = repo;
        _files = files;
        _dialogs = dialogs;
        Genders = new[]
        {
            new EnumOption<Gender>(Gender.Male, "ذكر"),
            new EnumOption<Gender>(Gender.Female, "أنثى"),
        };
        GuardianRelations = new[]
        {
            new EnumOption<GuardianRelation>(GuardianRelation.Father, "أب"),
            new EnumOption<GuardianRelation>(GuardianRelation.Mother, "أم"),
            new EnumOption<GuardianRelation>(GuardianRelation.Brother, "أخ"),
            new EnumOption<GuardianRelation>(GuardianRelation.Sister, "أخت"),
            new EnumOption<GuardianRelation>(GuardianRelation.Uncle, "عم/خال"),
            new EnumOption<GuardianRelation>(GuardianRelation.Aunt, "عمة/خالة"),
            new EnumOption<GuardianRelation>(GuardianRelation.Grandfather, "جد"),
            new EnumOption<GuardianRelation>(GuardianRelation.Grandmother, "جدة"),
            new EnumOption<GuardianRelation>(GuardianRelation.Other, "أخرى"),
        };
    }

    public ObservableCollection<GradeOption> Grades { get; } = new();
    public ObservableCollection<SectionOption> AvailableSections { get; } = new();
    public IReadOnlyList<EnumOption<Gender>> Genders { get; }
    public IReadOnlyList<EnumOption<GuardianRelation>> GuardianRelations { get; }

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string _title = "إضافة طالب";

    // Student fields
    [ObservableProperty] private string _studentNumber = string.Empty;
    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private Gender _gender = Gender.Male;
    [ObservableProperty] private DateTime _birthDate = new DateTime(2015, 1, 1);
    [ObservableProperty] private string? _nationalId;
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private string? _photoPath;
    [ObservableProperty] private string? _stagedPhotoSource;
    [ObservableProperty] private DateTime _enrollmentDate = DateTime.Today;
    [ObservableProperty] private GradeOption? _selectedGrade;
    [ObservableProperty] private SectionOption? _selectedSection;

    // Guardian fields
    [ObservableProperty] private string _guardianFullName = string.Empty;
    [ObservableProperty] private GuardianRelation _guardianRelation = GuardianRelation.Father;
    [ObservableProperty] private string? _guardianPhone;
    [ObservableProperty] private string? _guardianAltPhone;
    [ObservableProperty] private string? _guardianEmail;
    [ObservableProperty] private string? _guardianNationalId;
    [ObservableProperty] private string? _guardianOccupation;

    // Address & notes
    [ObservableProperty] private string? _address;
    [ObservableProperty] private string? _guardianAddress;
    [ObservableProperty] private string? _notes;

    public bool IsEditing => _editingId.HasValue;
    public string DisplayPhotoPath => StagedPhotoSource ?? PhotoPath ?? string.Empty;
    public bool HasPhoto => !string.IsNullOrEmpty(DisplayPhotoPath);

    public event EventHandler? Saved;
    public event EventHandler? Cancelled;

    public async Task LoadForCreateAsync(CancellationToken ct = default)
    {
        Reset();
        Title = "إضافة طالب";
        _editingId = null;
        await LoadLookupsAsync(ct).ConfigureAwait(true);
        StudentNumber = await _repo.NextStudentNumberAsync(ct).ConfigureAwait(true);
        EnrollmentDate = DateTime.Today;
    }

    public async Task LoadForEditAsync(int studentId, CancellationToken ct = default)
    {
        Reset();
        Title = "تعديل بيانات طالب";
        _editingId = studentId;
        await LoadLookupsAsync(ct).ConfigureAwait(true);

        var p = await _repo.GetForEditAsync(studentId, ct).ConfigureAwait(true);
        if (p is null)
        {
            ErrorMessage = "تعذّر العثور على الطالب.";
            return;
        }

        StudentNumber = p.StudentNumber;
        FullName = p.FullName;
        Gender = p.Gender;
        BirthDate = p.BirthDate == default ? new DateTime(2015, 1, 1) : p.BirthDate;
        NationalId = p.NationalId;
        Phone = p.Phone;
        PhotoPath = p.PhotoPath;
        EnrollmentDate = p.EnrollmentDate == default ? DateTime.Today : p.EnrollmentDate;
        Address = p.Address;
        Notes = p.Notes;

        var section = _allSections.FirstOrDefault(s => s.Id == p.SectionId);
        if (section is not null)
        {
            SelectedGrade = Grades.FirstOrDefault(g => g.Id == section.GradeId);
            // Setting Grade rebuilt AvailableSections — pick now.
            SelectedSection = AvailableSections.FirstOrDefault(s => s.Id == section.Id);
        }

        GuardianFullName = p.GuardianFullName;
        GuardianRelation = p.GuardianRelation;
        GuardianPhone = p.GuardianPhone;
        GuardianAltPhone = p.GuardianAltPhone;
        GuardianEmail = p.GuardianEmail;
        GuardianNationalId = p.GuardianNationalId;
        GuardianOccupation = p.GuardianOccupation;
        GuardianAddress = p.GuardianAddress;

        OnPropertyChanged(nameof(IsEditing));
    }

    private async Task LoadLookupsAsync(CancellationToken ct)
    {
        var lookups = await _repo.GetLookupsAsync(ct).ConfigureAwait(true);
        Grades.Clear();
        foreach (var g in lookups.Grades) Grades.Add(g);
        _allSections = lookups.Sections.ToList();
        AvailableSections.Clear();
    }

    partial void OnSelectedGradeChanged(GradeOption? value)
    {
        AvailableSections.Clear();
        if (value is null) { SelectedSection = null; return; }
        foreach (var s in _allSections.Where(s => s.GradeId == value.Id))
            AvailableSections.Add(s);
        if (SelectedSection is null || SelectedSection.GradeId != value.Id)
            SelectedSection = AvailableSections.FirstOrDefault();
    }

    partial void OnPhotoPathChanged(string? value)
    {
        OnPropertyChanged(nameof(DisplayPhotoPath));
        OnPropertyChanged(nameof(HasPhoto));
    }

    partial void OnStagedPhotoSourceChanged(string? value)
    {
        OnPropertyChanged(nameof(DisplayPhotoPath));
        OnPropertyChanged(nameof(HasPhoto));
    }

    [RelayCommand]
    private void PickPhoto()
    {
        var picked = _files.PickImage();
        if (!string.IsNullOrEmpty(picked))
            StagedPhotoSource = picked;
    }

    [RelayCommand]
    private void RemovePhoto()
    {
        StagedPhotoSource = null;
        PhotoPath = null;
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken ct)
    {
        ErrorMessage = null;
        var validation = Validate();
        if (validation is not null)
        {
            ErrorMessage = validation;
            return;
        }

        try
        {
            IsBusy = true;

            if (await _repo.StudentNumberExistsAsync(StudentNumber, _editingId, ct).ConfigureAwait(true))
            {
                ErrorMessage = "رقم الطالب مستخدم مسبقاً، اختر رقماً آخر.";
                return;
            }

            string? finalPhoto = PhotoPath;
            if (!string.IsNullOrEmpty(StagedPhotoSource))
                finalPhoto = await _files.SaveStudentPhotoAsync(StagedPhotoSource).ConfigureAwait(true);

            var model = new StudentSaveModel
            {
                Id = _editingId,
                StudentNumber = StudentNumber,
                FullName = FullName,
                Gender = Gender,
                BirthDate = BirthDate,
                NationalId = NationalId,
                Phone = Phone,
                PhotoPath = finalPhoto,
                EnrollmentDate = EnrollmentDate,
                Address = Address,
                Notes = Notes,
                SectionId = SelectedSection!.Id,
                GuardianId = null,
                GuardianFullName = GuardianFullName,
                GuardianRelation = GuardianRelation,
                GuardianPhone = GuardianPhone,
                GuardianAltPhone = GuardianAltPhone,
                GuardianEmail = GuardianEmail,
                GuardianNationalId = GuardianNationalId,
                GuardianOccupation = GuardianOccupation,
                GuardianAddress = GuardianAddress,
            };

            if (_editingId is null)
                await _repo.CreateAsync(model, ct).ConfigureAwait(true);
            else
                await _repo.UpdateAsync(model, ct).ConfigureAwait(true);

            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = "تعذّر حفظ الطالب: " + ex.Message;
            await _dialogs.ShowErrorAsync("خطأ في الحفظ", ex.Message).ConfigureAwait(true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel() => Cancelled?.Invoke(this, EventArgs.Empty);

    private string? Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName)) return "اسم الطالب مطلوب.";
        if (string.IsNullOrWhiteSpace(StudentNumber)) return "رقم الطالب مطلوب.";
        if (BirthDate == default || BirthDate.Year < 1990 || BirthDate > DateTime.Today)
            return "تاريخ الميلاد غير صالح.";
        if (SelectedGrade is null) return "اختر الصف.";
        if (SelectedSection is null) return "اختر الشعبة.";
        if (string.IsNullOrWhiteSpace(GuardianFullName)) return "اسم ولي الأمر مطلوب.";
        if (EnrollmentDate == default) return "تاريخ التسجيل غير صالح.";
        return null;
    }

    private void Reset()
    {
        ErrorMessage = null;
        StudentNumber = string.Empty;
        FullName = string.Empty;
        Gender = Gender.Male;
        BirthDate = new DateTime(2015, 1, 1);
        NationalId = null;
        Phone = null;
        PhotoPath = null;
        StagedPhotoSource = null;
        EnrollmentDate = DateTime.Today;
        SelectedGrade = null;
        SelectedSection = null;
        GuardianFullName = string.Empty;
        GuardianRelation = GuardianRelation.Father;
        GuardianPhone = null;
        GuardianAltPhone = null;
        GuardianEmail = null;
        GuardianNationalId = null;
        GuardianOccupation = null;
        Address = null;
        GuardianAddress = null;
        Notes = null;
        AvailableSections.Clear();
    }
}

public sealed record EnumOption<T>(T Value, string Label) where T : struct, Enum;
