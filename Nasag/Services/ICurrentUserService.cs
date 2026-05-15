using System;
using Nasag.Models;

namespace Nasag.Services;

public interface ICurrentUserService
{
    User? User { get; }
    bool IsAuthenticated { get; }

    /// <summary>The display name shown across the shell. Falls back to "مستخدم" when not signed in.</summary>
    string DisplayName { get; }

    /// <summary>Single uppercase letter used in the avatar circle.</summary>
    string Initial { get; }

    event EventHandler? SignedIn;
    event EventHandler? SignedOut;

    void SignIn(User user);
    void SignOut();
}
