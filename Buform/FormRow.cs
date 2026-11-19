namespace Buform;

internal sealed class FormRow
{
    public FormRowType Type { get; }

    public IFormGroup Group { get; }

    public IFormItem? Item { get; }

    public int ItemIndex { get; }

    public int GroupItemCount { get; }

    public IDisposable Data => (IDisposable?)Item ?? Group;

    private FormRow(FormRowType type, IFormGroup group, IFormItem? item, int itemIndex)
    {
        Type = type;
        Group = group;
        Item = item;
        ItemIndex = itemIndex;

        GroupItemCount = group.Count;
    }

    public static FormRow Header(IFormGroup group)
    {
        return new FormRow(FormRowType.Header, group, null, 0);
    }

    public static FormRow ForItem(IFormGroup group, IFormItem item, int itemIndex)
    {
        return new FormRow(FormRowType.Item, group, item, itemIndex);
    }

    public static FormRow Footer(IFormGroup group)
    {
        return new FormRow(FormRowType.Footer, group, null, 0);
    }
}
