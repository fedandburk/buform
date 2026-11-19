using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class AsyncPickerFormViewHolder<TItem> : PresentedPickerFormViewHolder<TItem>
    where TItem : class, IAsyncPickerFormItem
{
    protected AsyncPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected AsyncPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override PickerDialogFragment CreatePickerDialogFragment(TItem item)
    {
        return new AsyncPickerDialogFragment(item);
    }
}

public class AsyncPickerFormViewHolder : AsyncPickerFormViewHolder<IAsyncPickerFormItem>
{
    public AsyncPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public AsyncPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
