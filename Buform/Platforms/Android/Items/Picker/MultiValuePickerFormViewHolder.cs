using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class MultiValuePickerFormViewHolder
    : PresentedPickerFormViewHolderBase<IMultiValuePickerFormItem>
{
    public MultiValuePickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public MultiValuePickerFormViewHolder(View itemView)
        : base(itemView) { }

    protected override PickerPresenterBase<IMultiValuePickerFormItem>? GetPickerPresenter(
        PickerInputType inputType
    )
    {
        return inputType switch
        {
            PickerInputType.Default
                => new DialogFragmentPickerPresenter<IMultiValuePickerFormItem>(
                    CreatePickerDialogFragment
                ),
            PickerInputType.Dialog
                => new DialogFragmentPickerPresenter<IMultiValuePickerFormItem>(
                    CreatePickerDialogFragment
                ),
            PickerInputType.PopUp
                => new DialogFragmentPickerPresenter<IMultiValuePickerFormItem>(
                    CreatePickerDialogFragment
                ),
            _ => null
        };
    }
}
