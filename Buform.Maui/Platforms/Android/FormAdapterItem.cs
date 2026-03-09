namespace Buform;

internal enum FormAdapterItemKind
{
    SectionHeader = 0,
    Row = 1
}

internal sealed class FormAdapterItem
{
    public FormAdapterItemKind Kind { get; }
    public string? Title { get; }
    public object? Item { get; }

    private FormAdapterItem(FormAdapterItemKind kind, string? title, object? item)
    {
        Kind = kind;
        Title = title;
        Item = item;
    }

    public static FormAdapterItem CreateSectionHeader(string? title)
        => new(FormAdapterItemKind.SectionHeader, title, null);

    public static FormAdapterItem CreateRow(object item)
        => new(FormAdapterItemKind.Row, null, item);
}

