using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class PresentedPickerFormViewHolder<TFormItem> : PickerFormViewHolder<TFormItem>
    where TFormItem : class, IOptionsPickerFormItem
{
    protected virtual PickerPresenter<TFormItem>? PickerPresenter { get; set; }

    protected PresentedPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected PresentedPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override string? GetLabel(TFormItem item)
    {
        return item.Label;
    }

    protected override string? GetValue(TFormItem item)
    {
        return item.FormattedValue;
    }

    protected override void OnClick()
    {
        if (Data == null || PickerPresenter == null)
        {
            return;
        }

        PickerPresenter.Pick(ItemView, Data);
    }

    protected virtual PickerDialogFragment CreatePickerDialogFragment(TFormItem item)
    {
        return new PickerDialogFragment(item);
    }

    protected virtual PickerPresenter<TFormItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.Default => new DialogFragmentPickerPresenter<TFormItem>(
                CreatePickerDialogFragment
            ),
            PickerInputType.Dialog => new DialogFragmentPickerPresenter<TFormItem>(
                CreatePickerDialogFragment
            ),
            _ => null,
        };
    }

    protected virtual void UpdateInputType()
    {
        if (Data == null)
        {
            return;
        }

        var inputType = Data.InputType;

        PickerPresenter?.Dispose();
        PickerPresenter = GetPickerPresenter(inputType)
            ?? throw new ArgumentOutOfRangeException(
                nameof(IOptionsPickerFormItem.InputType),
                inputType,
                null
            );
    }

    protected override void OnDataSet()
    {
        base.OnDataSet();

        UpdateInputType();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        base.OnDataPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(IOptionsPickerFormItem.InputType):
            case null:
                UpdateInputType();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PickerPresenter?.Dispose();
            PickerPresenter = null;
        }

        base.Dispose(disposing);
    }
}
