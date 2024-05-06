using ObjCRuntime;

namespace Buform;

public abstract class PresentedPickerFormCellBase<TFormItem> : PickerFormCellBase<TFormItem>
    where TFormItem : class, IPickerFormItemBase
{
    protected virtual PickerPresenterBase<TFormItem>? PickerPresenter { get; set; }

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected PresentedPickerFormCellBase()
    {
        /* Required constructor */
    }

    protected PresentedPickerFormCellBase(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual UIViewController CreateViewController(TFormItem item)
    {
        return new PickerViewController<TFormItem>(UITableViewStyle.InsetGrouped, item);
    }

    protected virtual void UpdateInputType()
    {
        if (Item == null)
        {
            return;
        }

        PickerPresenter?.Dispose();

        PickerPresenter = GetPickerPresenter(Item.InputType);

        if (PickerPresenter is null)
        {
            throw new ArgumentOutOfRangeException(nameof(Item.InputType), Item.InputType, null);
        }
    }

    protected virtual PickerPresenterBase<TFormItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.Default => new DefaultPickerPresenter<TFormItem>(CreateViewController),
            PickerInputType.Dialog => new DialogPickerPresenter<TFormItem>(CreateViewController),
            _ => default
        };
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel(Item?.Label);
        UpdateInputType();
        UpdateValue(Item?.FormattedValue);
        UpdateValidationErrorMessage(Item?.ValidationErrorMessage);
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Item.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(Item.Label):
                UpdateLabel(Item?.Label);
                break;
            case nameof(Item.InputType):
                UpdateInputType();
                break;
            case nameof(Item.FormattedValue):
                UpdateValue(Item?.FormattedValue);
                break;
            case nameof(Item.ValidationErrorMessage):
                UpdateValidationErrorMessage(Item?.ValidationErrorMessage);
                break;
        }
    }

    public override async void OnSelected()
    {
        base.OnSelected();

        if (Item == null)
        {
            return;
        }

        if (PickerPresenter == null)
        {
            return;
        }

        await PickerPresenter.PickAsync(this, Item).ConfigureAwait(true);
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
