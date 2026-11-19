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

    protected override PickerPresenter<TItem> CreatePickerPresenter(PickerPresentation presentation)
    {
        return presentation == PickerPresentation.PopUp
            ? new PopUpPickerPresenter<TItem>()
            : base.CreatePickerPresenter(presentation);
    }
}

public sealed class PopUpPickerFormViewHolder : PopUpPickerFormViewHolder<IPickerFormItem>
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
