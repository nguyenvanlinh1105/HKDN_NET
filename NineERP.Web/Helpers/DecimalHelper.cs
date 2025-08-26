namespace NineERP.Web.Helpers;

public static class DecimalHelper
{
    public static string FormatDecimal(decimal? value)
    {
        if (!value.HasValue) return string.Empty;

        // Use ToString("G29") to remove the zeros at the end of the decimal part
        return value.Value.ToString("G29");
    }
}