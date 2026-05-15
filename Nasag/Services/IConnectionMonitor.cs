using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nasag.Services;

public interface IConnectionMonitor
{
    bool IsConnected { get; }
    string? LastErrorMessage { get; }
    event EventHandler? StateChanged;

    Task<bool> CheckAsync();
    void ReportFailure(string message);
    void ReportSuccess();
}

/// <summary>
/// Phase 2 stub. Phase 3 will wire it to an actual DbContext.CanConnectAsync check.
/// </summary>
public sealed partial class ConnectionMonitor : ObservableObject, IConnectionMonitor
{
    [ObservableProperty]
    private bool _isConnected = true;

    [ObservableProperty]
    private string? _lastErrorMessage;

    public event EventHandler? StateChanged;

    public Task<bool> CheckAsync()
    {
        // In Phase 3 this will attempt NasaqDbContext.Database.CanConnectAsync().
        return Task.FromResult(IsConnected);
    }

    public void ReportFailure(string message)
    {
        LastErrorMessage = message;
        if (IsConnected)
        {
            IsConnected = false;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ReportSuccess()
    {
        LastErrorMessage = null;
        if (!IsConnected)
        {
            IsConnected = true;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
