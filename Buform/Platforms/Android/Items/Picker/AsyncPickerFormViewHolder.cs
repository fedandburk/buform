using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class AsyncPickerFormViewHolder<TItem> : PresentedPickerFormViewHolderBase<TItem>
    where TItem : class, IAsyncPickerFormItem
{
    protected AsyncPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    protected AsyncPickerFormViewHolder(View itemView)
        : base(itemView) { }

    protected override PickerDialogFragment CreatePickerDialogFragment(TItem item)
    {
        return AsyncPickerDialogFragment.Create(item);
    }
}

[Preserve(AllMembers = true)]
[Register(nameof(AsyncPickerFormViewHolder))]
public sealed class AsyncPickerFormViewHolder : AsyncPickerFormViewHolder<IAsyncPickerFormItem>
{
    public AsyncPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public AsyncPickerFormViewHolder(View itemView)
        : base(itemView) { }
}
