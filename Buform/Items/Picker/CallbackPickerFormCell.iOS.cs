using ObjCRuntime;

namespace Buform;

public abstract class CallbackPickerFormCell<TItem> : PickerFormCell<TItem>
    where TItem : class, ICallbackPickerFormItem
{
    protected CallbackPickerFormCell()
    {
        /* Required constructor */
    }

    protected CallbackPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public override async void OnSelected()
    {
        base.OnSelected();

        if (Item == null)
        {
            return;
        }

        await Item.ExecuteCallbackAsync().ConfigureAwait(true);
    }

    protected override string? GetLabel(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Label;
    }

    protected override string? GetValue(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.FormattedValue;
    }
}

[Register(nameof(CallbackPickerFormCell))]
public sealed class CallbackPickerFormCell : CallbackPickerFormCell<ICallbackPickerFormItem>
{
    public CallbackPickerFormCell()
    {
        /* Required constructor */
    }

    public CallbackPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
