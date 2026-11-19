using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class MultiValuePickerFormViewHolder<TItem> : PresentedPickerFormViewHolder<TItem>
    where TItem : class, IMultiValuePickerFormItem
{
    protected MultiValuePickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected MultiValuePickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}

public sealed class MultiValuePickerFormViewHolder
    : MultiValuePickerFormViewHolder<IMultiValuePickerFormItem>
{
    public MultiValuePickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public MultiValuePickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
