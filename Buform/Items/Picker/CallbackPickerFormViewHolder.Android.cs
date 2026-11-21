using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class CallbackPickerFormViewHolder<TItem> : PickerFormViewHolder<TItem>
    where TItem : class, ICallbackPickerFormItem
{
    protected CallbackPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected CallbackPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
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

    protected override Task OnClickAsync()
    {
        if (Item == null)
        {
            throw new InvalidOperationException("The picker row has no item.");
        }

        return Item.ExecuteCallbackAsync();
    }
}

public sealed class CallbackPickerFormViewHolder
    : CallbackPickerFormViewHolder<ICallbackPickerFormItem>
{
    public CallbackPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public CallbackPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
