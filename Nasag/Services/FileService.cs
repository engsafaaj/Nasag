using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Nasag.Services;

public sealed class FileService : IFileService
{
    private static readonly string PhotosRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Nasaq", "Photos", "Students");

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

    public async Task<string?> SaveStudentPhotoAsync(string? sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            return null;

        Directory.CreateDirectory(PhotosRoot);

        var ext = Path.GetExtension(sourcePath);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var destination = Path.Combine(PhotosRoot, fileName);

        await using var src = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using var dst = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await src.CopyToAsync(dst).ConfigureAwait(false);

        return destination;
    }
}
