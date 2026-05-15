namespace Nasag.Services;

public sealed class UserPreferences
{
    public string? RememberedUsername { get; set; }
    public bool StudentsSortAlphabetically { get; set; } = true;
    public int StudentsPageSize { get; set; } = 20;
}

public interface IUserPreferencesService
{
    UserPreferences Current { get; }
    void Save();
    void Reload();
}
