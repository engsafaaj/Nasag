namespace Nasag.Models;

public class SchoolSettings
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? PrincipalName { get; set; }

    public int? CurrentAcademicYearId { get; set; }
    public AcademicYear? CurrentAcademicYear { get; set; }
}
