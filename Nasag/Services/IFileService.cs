using System.Threading.Tasks;

namespace Nasag.Services;

public interface IFileService
{
    /// <summary>Opens a file picker for image files. Returns the selected absolute path or null if cancelled.</summary>
    string? PickImage();

    /// <summary>
    /// Copies an external image into the application's local photos folder, returning the new absolute path.
    /// The original file is left untouched. Returns null if the source path is null/empty/missing.
    /// </summary>
    Task<string?> SaveStudentPhotoAsync(string? sourcePath);
}
