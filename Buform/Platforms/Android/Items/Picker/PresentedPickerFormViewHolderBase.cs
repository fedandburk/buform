using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class PresentedPickerFormViewHolderBase<TFormItem>
    : PickerFormViewHolderBase<TFormItem>
    where TFormItem : class, IPickerFormItemBase
{
    protected virtual PickerPresenterBase<TFormItem>? PickerPresenter { get; set; }

    public virtual bool IsSelectable => !(Data?.IsReadOnly ?? true);

    protected PresentedPickerFormViewHolderBase(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    protected PresentedPickerFormViewHolderBase(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        base.Initialize();
        AttachListeners();
    }

    protected virtual void AttachListeners()
    {
        ItemView.Click += OnItemViewClick;
    }

    protected virtual void DetachListeners()
    {
        ItemView.Click -= OnItemViewClick;
    }

    private void OnItemViewClick(object? sender, EventArgs e)
    {
        HandleClick();
    }

    protected virtual void HandleClick()
    {
        if (!IsSelectable || Data == null || PickerPresenter == null)
        {
            return;
        }

        PickerPresenter.Pick(ItemView, Data);
    }

    protected virtual PickerDialogFragment CreatePickerDialogFragment(TFormItem item)
    {
        return PickerDialogFragment.Create(item);
    }

    protected virtual void UpdateInputType()
    {
        if (Data == null)
        {
            return;
        }

        PickerPresenter?.Dispose();
        PickerPresenter = GetPickerPresenter(Data.InputType);

        if (PickerPresenter == null)
        {
            throw new ArgumentOutOfRangeException(nameof(Data.InputType), Data.InputType, null);
        }
    }

    protected virtual PickerPresenterBase<TFormItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.Default
                => new DialogFragmentPickerPresenter<TFormItem>(CreatePickerDialogFragment),
            PickerInputType.Dialog
                => new DialogFragmentPickerPresenter<TFormItem>(CreatePickerDialogFragment),
            _ => null
        };
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel(Data?.Label);
        UpdateInputType();
        UpdateValue(Data?.FormattedValue);
        UpdateValidationErrorMessage(Data?.ValidationErrorMessage);
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(IPickerFormItemBase.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(IPickerFormItemBase.Label):
                UpdateLabel(Data?.Label);
                break;
            case nameof(IPickerFormItemBase.InputType):
                UpdateInputType();
                break;
            case nameof(IPickerFormItemBase.FormattedValue):
                UpdateValue(Data?.FormattedValue);
                break;
            case nameof(IPickerFormItemBase.ValidationErrorMessage):
                UpdateValidationErrorMessage(Data?.ValidationErrorMessage);
                break;
            default:
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachListeners();

            PickerPresenter?.Dispose();
            PickerPresenter = null;
        }

        base.Dispose(disposing);
    }
}
