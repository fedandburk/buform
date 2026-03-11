namespace Buform;

internal static class FormReflectionHelper
{
    public static string? GetHeaderLabel(object item) =>
        item.GetType().GetProperty(FormReflectionKeys.HeaderLabel)?.GetValue(item) as string;

    public static string? GetFooterLabel(object item) =>
        item.GetType().GetProperty(FormReflectionKeys.FooterLabel)?.GetValue(item) as string;

    public static string? GetLabel(object item) =>
        item.GetType().GetProperty(FormReflectionKeys.Label)?.GetValue(item) as string;

    public static string? GetFormattedValue(object item) =>
        item.GetType().GetProperty(FormReflectionKeys.FormattedValue)?.GetValue(item) as string;
}
