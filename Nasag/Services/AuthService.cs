using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nasag.Data;
using Nasag.Models;

namespace Nasag.Services;

public sealed class AuthService : IAuthService
{
    private readonly IDbContextFactory<NasaqDbContext> _factory;

    public AuthService(IDbContextFactory<NasaqDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<AuthResult> SignInAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return AuthResult.Fail(AuthFailureReason.InvalidCredentials, "اسم المستخدم وكلمة المرور مطلوبان.");

        try
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);

            var user = await ctx.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username, ct)
                .ConfigureAwait(false);

            if (user is null)
                return AuthResult.Fail(AuthFailureReason.InvalidCredentials, "اسم المستخدم أو كلمة المرور غير صحيحة.");

            if (!user.IsActive)
                return AuthResult.Fail(AuthFailureReason.AccountDisabled, "هذا الحساب موقوف. يرجى التواصل مع المدير.");

            bool verified;
            try
            {
                verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                verified = false;
            }

            if (!verified)
                return AuthResult.Fail(AuthFailureReason.InvalidCredentials, "اسم المستخدم أو كلمة المرور غير صحيحة.");

            user.LastLoginAt = DateTime.UtcNow;
            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

            return AuthResult.Ok(user);
        }
        catch (SqlException ex)
        {
            return AuthResult.Fail(
                AuthFailureReason.ConnectionError,
                "تعذّر الاتصال بقاعدة البيانات. تحقق من الاتصال وحاول مجدداً." + Environment.NewLine + ex.Message);
        }
        catch (Exception ex)
        {
            return AuthResult.Fail(AuthFailureReason.Unknown, "حدث خطأ غير متوقع: " + ex.Message);
        }
    }
}
