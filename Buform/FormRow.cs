namespace Buform;

internal sealed class FormRow
{
    public FormRowType Type { get; }

    public IFormGroup Group { get; }

    public IFormItem? Item { get; }

    public IDisposable Data => (IDisposable?)Item ?? Group;

    private FormRow(FormRowType type, IFormGroup group, IFormItem? item)
    {
        Type = type;
        Group = group;
        Item = item;
    }

    public static FormRow Header(IFormGroup group)
    {
        return new FormRow(FormRowType.Header, group, null);
    }

    public static FormRow ForItem(IFormGroup group, IFormItem item)
    {
        return new FormRow(FormRowType.Item, group, item);
    }

    public static FormRow Footer(IFormGroup group)
    {
        return new FormRow(FormRowType.Footer, group, null);
    }
}
