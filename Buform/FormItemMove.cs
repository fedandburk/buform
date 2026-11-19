namespace Buform;

public sealed class FormItemMove
{
    public object? Value { get; }

    public int OldIndex { get; }

    public int NewIndex { get; }

    public FormItemMove(object? value, int oldIndex, int newIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(oldIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(newIndex);

        Value = value;
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    internal static FormItemMove Create(IFormGroup group, int oldVisibleIndex, int newVisibleIndex)
    {
        ArgumentNullException.ThrowIfNull(group);

        var item = group[oldVisibleIndex];
        var targetItem = group[newVisibleIndex];

        return new FormItemMove(
            item.Value,
            group.GetSourceIndex(item),
            group.GetSourceIndex(targetItem)
        );
    }
}
