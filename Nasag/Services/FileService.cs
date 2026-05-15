using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Nasag.Services;

public sealed class FileService : IFileService
{
    public string? PickImage()
    {
        var dlg = new OpenFileDialog
        {
            Title = "اختر صورة الطالب",
            Filter = "صور (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|كل الملفات (*.*)|*.*",
            Multiselect = false,
            CheckFileExists = true,
        };
        return dlg.ShowDialog() == true ? dlg.FileName : null;
    }

    public async Task<byte[]?> ReadAllBytesAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return null;
        return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
    }
}
