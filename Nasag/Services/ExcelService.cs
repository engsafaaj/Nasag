using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Nasag.Services;

/// <summary>
/// .xlsx writer/reader for the Students Import/Export feature.
/// Headers are Arabic and frozen; columns auto-fit; banded rows for legibility.
/// </summary>
public sealed class ExcelService : IExcelService
{
    private static readonly string[] Headers =
    {
        "رقم الطالب",
        "الاسم الكامل",
        "الجنس",
        "تاريخ الميلاد",
        "رقم الهوية",
        "جوال الطالب",
        "الصف",
        "الشعبة",
        "تاريخ التسجيل",
        "الحالة",
        "عنوان الطالب",
        "اسم ولي الأمر",
        "صلة القرابة",
        "جوال ولي الأمر",
        "جوال احتياطي",
        "البريد الإلكتروني",
        "هوية ولي الأمر",
        "مهنة ولي الأمر",
        "عنوان ولي الأمر",
        "ملاحظات",
    };

    public Task ExportStudentsAsync(string filePath, IReadOnlyList<StudentExportRow> rows, CancellationToken ct = default)
        => Task.Run(() => ExportInternal(filePath, rows, ct), ct);

    public Task<IReadOnlyList<StudentImportRow>> ReadStudentsAsync(string filePath, CancellationToken ct = default)
        => Task.Run(() => ReadInternal(filePath, ct), ct);

    public Task WriteTemplateAsync(string filePath, CancellationToken ct = default)
        => Task.Run(() => ExportInternal(filePath, Array.Empty<StudentExportRow>(), ct), ct);

    private static void ExportInternal(string filePath, IReadOnlyList<StudentExportRow> rows, CancellationToken ct)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("الطلاب");
        ws.RightToLeft = true;

        // Header
        for (var i = 0; i < Headers.Length; i++)
            ws.Cell(1, i + 1).Value = Headers[i];
        var headerRange = ws.Range(1, 1, 1, Headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1FB5A8");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.SheetView.FreezeRows(1);
        ws.Row(1).Height = 24;

        // Body
        for (var r = 0; r < rows.Count; r++)
        {
            ct.ThrowIfCancellationRequested();
            var row = rows[r];
            var i = r + 2; // 1-based, header row is 1
            ws.Cell(i, 1).Value = row.StudentNumber;
            ws.Cell(i, 2).Value = row.FullName;
            ws.Cell(i, 3).Value = row.Gender;
            ws.Cell(i, 4).Value = row.BirthDate;
            ws.Cell(i, 5).Value = row.NationalId;
            ws.Cell(i, 6).Value = row.Phone;
            ws.Cell(i, 7).Value = row.GradeName;
            ws.Cell(i, 8).Value = row.SectionName;
            ws.Cell(i, 9).Value = row.EnrollmentDate;
            ws.Cell(i, 10).Value = row.Status;
            ws.Cell(i, 11).Value = row.Address;
            ws.Cell(i, 12).Value = row.GuardianFullName;
            ws.Cell(i, 13).Value = row.GuardianRelation;
            ws.Cell(i, 14).Value = row.GuardianPhone;
            ws.Cell(i, 15).Value = row.GuardianAltPhone;
            ws.Cell(i, 16).Value = row.GuardianEmail;
            ws.Cell(i, 17).Value = row.GuardianNationalId;
            ws.Cell(i, 18).Value = row.GuardianOccupation;
            ws.Cell(i, 19).Value = row.GuardianAddress;
            ws.Cell(i, 20).Value = row.Notes;
        }

        if (rows.Count > 0)
        {
            var body = ws.Range(2, 1, rows.Count + 1, Headers.Length);
            body.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            body.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            body.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            body.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            body.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CFD6E0");
            body.Style.Border.InsideBorderColor = XLColor.FromHtml("#E5E9F0");
        }

        ws.Columns().AdjustToContents(minWidth: 12, maxWidth: 36);
        ws.SheetView.View = XLSheetViewOptions.Normal;

        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
        wb.SaveAs(filePath);
    }

    private static IReadOnlyList<StudentImportRow> ReadInternal(string filePath, CancellationToken ct)
    {
        using var wb = new XLWorkbook(filePath);
        var ws = wb.Worksheets.Worksheet(1);
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        var result = new List<StudentImportRow>(Math.Max(0, lastRow - 1));

        // Build a header→column lookup so import is resilient to column re-ordering.
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var headerRow = ws.Row(1);
        for (var c = 1; c <= 20; c++)
        {
            var key = headerRow.Cell(c).GetString().Trim();
            if (!string.IsNullOrEmpty(key) && !map.ContainsKey(key)) map[key] = c;
        }

        string? Get(IXLRow row, string headerKey)
        {
            if (!map.TryGetValue(headerKey, out var col)) return null;
            var cell = row.Cell(col);
            var s = cell.IsEmpty() ? null : cell.GetString().Trim();
            return string.IsNullOrEmpty(s) ? null : s;
        }

        for (var r = 2; r <= lastRow; r++)
        {
            ct.ThrowIfCancellationRequested();
            var row = ws.Row(r);
            if (row.IsEmpty()) continue;

            result.Add(new StudentImportRow
            {
                RowNumber = r,
                StudentNumber = Get(row, "رقم الطالب"),
                FullName = Get(row, "الاسم الكامل"),
                Gender = Get(row, "الجنس"),
                BirthDate = Get(row, "تاريخ الميلاد"),
                NationalId = Get(row, "رقم الهوية"),
                Phone = Get(row, "جوال الطالب"),
                GradeName = Get(row, "الصف"),
                SectionName = Get(row, "الشعبة"),
                EnrollmentDate = Get(row, "تاريخ التسجيل"),
                Address = Get(row, "عنوان الطالب"),
                GuardianFullName = Get(row, "اسم ولي الأمر"),
                GuardianRelation = Get(row, "صلة القرابة"),
                GuardianPhone = Get(row, "جوال ولي الأمر"),
                GuardianAltPhone = Get(row, "جوال احتياطي"),
                GuardianEmail = Get(row, "البريد الإلكتروني"),
                GuardianNationalId = Get(row, "هوية ولي الأمر"),
                GuardianOccupation = Get(row, "مهنة ولي الأمر"),
                GuardianAddress = Get(row, "عنوان ولي الأمر"),
                Notes = Get(row, "ملاحظات"),
            });
        }

        return result;
    }
}
