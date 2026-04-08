namespace Buform;

internal sealed class FormAdapterItem
{
    public FormViewHolderType HolderType { get; }

    public object Item { get; }

    private FormAdapterItem(FormViewHolderType holderType, object? item)
    {
        ArgumentNullException.ThrowIfNull(item);
        HolderType = holderType;
        Item = item;
    }

    public static FormAdapterItem CreateSectionHeader(object section) =>
        new(FormViewHolderType.Header, section);

    public static FormAdapterItem CreateRow(object item) => new(FormViewHolderType.Item, item);

    public static FormAdapterItem CreateSectionFooter(object section) =>
        new(FormViewHolderType.Footer, section);
}
