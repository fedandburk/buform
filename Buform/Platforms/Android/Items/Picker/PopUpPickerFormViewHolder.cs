using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class PopUpPickerFormViewHolder<TItem> : PresentedPickerFormViewHolderBase<TItem>
    where TItem : class, IPickerFormItem
{
    protected PopUpPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    protected PopUpPickerFormViewHolder(View itemView)
        : base(itemView) { }

    protected override PickerPresenterBase<TItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.PopUp
                => new DialogFragmentPickerPresenter<TItem>(CreatePickerDialogFragment),
            _ => base.GetPickerPresenter(inputType)
        };
    }
}

[Preserve(AllMembers = true)]
[Register(nameof(PopUpPickerFormViewHolder))]
public sealed class PopUpPickerFormViewHolder : PopUpPickerFormViewHolder<IPickerFormItem>
{
    public PopUpPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public PopUpPickerFormViewHolder(View itemView)
        : base(itemView) { }
}
