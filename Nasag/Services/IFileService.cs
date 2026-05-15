using System.Threading.Tasks;

namespace Nasag.Services;

public interface IFileService
{
    /// <summary>Opens a file picker for image files. Returns the selected absolute path or null if cancelled.</summary>
    string? PickImage();

    /// <summary>Reads the file at the given path into memory. Returns null if the path is missing/empty/invalid.</summary>
    Task<byte[]?> ReadAllBytesAsync(string? path);
}
