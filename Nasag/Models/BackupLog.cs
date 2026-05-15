using System;

namespace Nasag.Models;

public class BackupLog
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long SizeBytes { get; set; }
    public string? Notes { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
