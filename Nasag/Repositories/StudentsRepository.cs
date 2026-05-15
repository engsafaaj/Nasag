using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nasag.Data;
using Nasag.Models;

namespace Nasag.Repositories;

public sealed class StudentsRepository : IStudentsRepository
{
    private readonly IDbContextFactory<NasaqDbContext> _factory;

    public StudentsRepository(IDbContextFactory<NasaqDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<StudentListPage> SearchAsync(StudentsQuery query, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);

        var q = ctx.Students
            .AsNoTracking()
            .Include(s => s.Section).ThenInclude(sec => sec.Grade)
            .Include(s => s.Guardian)
            .AsQueryable();

        if (query.Status.HasValue)
            q = q.Where(s => s.Status == query.Status.Value);

        if (query.SectionId.HasValue)
            q = q.Where(s => s.SectionId == query.SectionId.Value);
        else if (query.GradeId.HasValue)
            q = q.Where(s => s.Section.GradeId == query.GradeId.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(s =>
                s.FullName.Contains(term) ||
                s.StudentNumber.Contains(term) ||
                (s.NationalId != null && s.NationalId.Contains(term)) ||
                (s.Phone != null && s.Phone.Contains(term)) ||
                s.Guardian.FullName.Contains(term));
        }

        var total = await q.CountAsync(ct).ConfigureAwait(false);

        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.PageSize, 1, 200);

        var rows = await q
            .OrderBy(s => s.FullName)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(s => new StudentRow(
                s.Id,
                s.StudentNumber,
                s.FullName,
                s.Gender,
                s.Status,
                s.Section.Grade.NameAr,
                s.Section.NameAr,
                s.Phone,
                s.Guardian.FullName,
                s.Guardian.Phone))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new StudentListPage(rows, total, page, size);
    }

    public async Task<StudentStats> GetStatsAsync(CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var groups = await ctx.Students
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct)
            .ConfigureAwait(false);

        int Get(StudentStatus s) => groups.FirstOrDefault(g => g.Status == s)?.Count ?? 0;
        var active = Get(StudentStatus.Active);
        var archived = Get(StudentStatus.Archived);
        var graduated = Get(StudentStatus.Graduated);
        return new StudentStats(active + archived + graduated, active, archived);
    }

    public async Task<StudentEditorLookups> GetLookupsAsync(CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);

        var grades = await ctx.Grades
            .AsNoTracking()
            .OrderBy(g => g.SortOrder)
            .Select(g => new GradeOption(g.Id, g.NameAr))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var sections = await ctx.Sections
            .AsNoTracking()
            .OrderBy(s => s.GradeId).ThenBy(s => s.NameAr)
            .Select(s => new SectionOption(s.Id, s.NameAr, s.GradeId))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new StudentEditorLookups(grades, sections);
    }

    public async Task<StudentEditorPayload?> GetForEditAsync(int studentId, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);

        return await ctx.Students
            .AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => new StudentEditorPayload(
                s.Id,
                s.StudentNumber,
                s.FullName,
                s.Gender,
                s.BirthDate,
                s.NationalId,
                s.PhotoPath,
                s.Phone,
                s.Address,
                s.Notes,
                s.EnrollmentDate,
                s.Status,
                s.SectionId,
                s.GuardianId,
                s.Guardian.FullName,
                s.Guardian.Relation,
                s.Guardian.Phone,
                s.Guardian.AltPhone,
                s.Guardian.Email,
                s.Guardian.NationalId,
                s.Guardian.Occupation,
                s.Guardian.Address))
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<int> CreateAsync(StudentSaveModel model, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        await using var tx = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

        var guardian = new Guardian
        {
            FullName = model.GuardianFullName.Trim(),
            Relation = model.GuardianRelation,
            Phone = NullIfEmpty(model.GuardianPhone),
            AltPhone = NullIfEmpty(model.GuardianAltPhone),
            Email = NullIfEmpty(model.GuardianEmail),
            NationalId = NullIfEmpty(model.GuardianNationalId),
            Occupation = NullIfEmpty(model.GuardianOccupation),
            Address = NullIfEmpty(model.GuardianAddress),
        };
        ctx.Guardians.Add(guardian);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        var student = new Student
        {
            StudentNumber = model.StudentNumber.Trim(),
            FullName = model.FullName.Trim(),
            Gender = model.Gender,
            BirthDate = model.BirthDate.Date,
            NationalId = NullIfEmpty(model.NationalId),
            PhotoPath = NullIfEmpty(model.PhotoPath),
            Phone = NullIfEmpty(model.Phone),
            Address = NullIfEmpty(model.Address),
            Notes = NullIfEmpty(model.Notes),
            EnrollmentDate = model.EnrollmentDate.Date,
            Status = StudentStatus.Active,
            SectionId = model.SectionId,
            GuardianId = guardian.Id,
        };
        ctx.Students.Add(student);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        await tx.CommitAsync(ct).ConfigureAwait(false);
        return student.Id;
    }

    public async Task UpdateAsync(StudentSaveModel model, CancellationToken ct = default)
    {
        if (model.Id is null)
            throw new InvalidOperationException("UpdateAsync requires Id.");

        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        await using var tx = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

        var student = await ctx.Students
            .Include(s => s.Guardian)
            .FirstOrDefaultAsync(s => s.Id == model.Id, ct)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("الطالب غير موجود.");

        student.StudentNumber = model.StudentNumber.Trim();
        student.FullName = model.FullName.Trim();
        student.Gender = model.Gender;
        student.BirthDate = model.BirthDate.Date;
        student.NationalId = NullIfEmpty(model.NationalId);
        student.PhotoPath = NullIfEmpty(model.PhotoPath);
        student.Phone = NullIfEmpty(model.Phone);
        student.Address = NullIfEmpty(model.Address);
        student.Notes = NullIfEmpty(model.Notes);
        student.EnrollmentDate = model.EnrollmentDate.Date;
        student.SectionId = model.SectionId;

        var guardian = student.Guardian;
        guardian.FullName = model.GuardianFullName.Trim();
        guardian.Relation = model.GuardianRelation;
        guardian.Phone = NullIfEmpty(model.GuardianPhone);
        guardian.AltPhone = NullIfEmpty(model.GuardianAltPhone);
        guardian.Email = NullIfEmpty(model.GuardianEmail);
        guardian.NationalId = NullIfEmpty(model.GuardianNationalId);
        guardian.Occupation = NullIfEmpty(model.GuardianOccupation);
        guardian.Address = NullIfEmpty(model.GuardianAddress);

        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);
        await tx.CommitAsync(ct).ConfigureAwait(false);
    }

    public async Task SetStatusAsync(int studentId, StudentStatus status, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var student = await ctx.Students.FirstOrDefaultAsync(s => s.Id == studentId, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("الطالب غير موجود.");
        student.Status = status;
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<bool> StudentNumberExistsAsync(string studentNumber, int? excludeId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(studentNumber)) return false;
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var n = studentNumber.Trim();
        return excludeId is int id
            ? await ctx.Students.AnyAsync(s => s.StudentNumber == n && s.Id != id, ct).ConfigureAwait(false)
            : await ctx.Students.AnyAsync(s => s.StudentNumber == n, ct).ConfigureAwait(false);
    }

    public async Task<string> NextStudentNumberAsync(CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var lastNumeric = await ctx.Students
            .Select(s => s.StudentNumber)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var max = 20250000;
        foreach (var raw in lastNumeric)
        {
            if (int.TryParse(raw, out var n) && n > max) max = n;
        }
        return (max + 1).ToString();
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
