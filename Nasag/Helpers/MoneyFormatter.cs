using System.Globalization;

namespace Nasag.Helpers;

/// <summary>
/// Centralized currency formatter used across ViewModels and printable documents.
/// WHY: avoids " ر.س" string drift across the Fees module. To switch a school's
/// currency in the future, change one call-site only (or pass <c>currency</c>).
/// </summary>
public static class MoneyFormatter
{
    /// <summary>Default currency label used when none is provided.</summary>
    public const string DefaultCurrency = "ر.س";

    // WHY: ar-SA culture renders Arabic digits/grouping correctly in print previews
    // and keeps consistent thousand separators across machines that may not have
    // Arabic culture installed. Falls back gracefully to InvariantCulture if missing.
    private static readonly CultureInfo ArabicCulture = TryGetArabicCulture();

    public static string Format(decimal amount, string currency = DefaultCurrency)
    {
        var text = amount.ToString("N2", ArabicCulture);
        return string.IsNullOrWhiteSpace(currency) ? text : text + " " + currency;
    }

    private static CultureInfo TryGetArabicCulture()
    {
        try { return CultureInfo.GetCultureInfo("ar-SA"); }
        catch { return CultureInfo.InvariantCulture; }
    }
}
