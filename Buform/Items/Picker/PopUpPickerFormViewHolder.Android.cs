using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class PopUpPickerFormViewHolder<TItem> : PresentedPickerFormViewHolder<TItem>
    where TItem : class, IPickerFormItem
{
    protected PopUpPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected PopUpPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override PickerPresenter<TItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType == PickerInputType.PopUp
            ? new PopUpPickerPresenter<TItem>()
            : base.GetPickerPresenter(inputType);
    }
}

public class PopUpPickerFormViewHolder : PopUpPickerFormViewHolder<IPickerFormItem>
{
    public PopUpPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public PopUpPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
