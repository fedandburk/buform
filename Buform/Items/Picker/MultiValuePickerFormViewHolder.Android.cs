using Android.Runtime;
using Android.Views;

namespace Buform;

public class MultiValuePickerFormViewHolder
    : PresentedPickerFormViewHolder<IMultiValuePickerFormItem>
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
