namespace Nasag.Services;

public sealed class UserPreferences
{
    public string? RememberedUsername { get; set; }
    public bool StudentsSortAlphabetically { get; set; } = true;
    public int StudentsPageSize { get; set; } = 20;

    /// <summary>
    /// Phase 12 — remembered target folder for the next Backup &amp; Restore session.
    /// Null means "use the default" (<c>%LOCALAPPDATA%\Nasaq\Backups</c>). Per-machine
    /// like the rest of <see cref="UserPreferences"/>; not part of the school DB.
    /// </summary>
    public string? BackupFolder { get; set; }
}

public interface IUserPreferencesService
{
    UserPreferences Current { get; }
    void Save();
    void Reload();
}
