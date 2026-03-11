namespace Buform;

internal enum FormAdapterItemKind
{
    SectionHeader = 0,
    Row = 1,
    SectionFooter = 2,
}

internal sealed class FormAdapterItem
{
    public FormAdapterItemKind Kind { get; }

    public object? Item { get; }

    private FormAdapterItem(FormAdapterItemKind kind, object? item)
    {
        Kind = kind;
        Item = item;
    }

    public static FormAdapterItem CreateSectionHeader(object section) =>
        new(FormAdapterItemKind.SectionHeader, section);

    public static FormAdapterItem CreateRow(object item) => new(FormAdapterItemKind.Row, item);

    public static FormAdapterItem CreateSectionFooter(object section) =>
        new(FormAdapterItemKind.SectionFooter, section);
}
