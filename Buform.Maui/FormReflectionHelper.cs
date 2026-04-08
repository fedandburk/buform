namespace Buform;

internal static class FormReflectionHelper
{
    public static string? GetHeaderLabel(object item) =>
        GetStringProperty(item, FormReflectionKeys.HeaderLabel);

    public static string? GetFooterLabel(object item) =>
        GetStringProperty(item, FormReflectionKeys.FooterLabel);

    public static string? GetLabel(object item) =>
        GetStringProperty(item, FormReflectionKeys.Label);

    public static string? GetFormattedValue(object item) =>
        GetStringProperty(item, FormReflectionKeys.FormattedValue);

    private static string? GetStringProperty(object item, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(item);
        return item.GetType().GetProperty(propertyName)?.GetValue(item) as string;
    }
}
