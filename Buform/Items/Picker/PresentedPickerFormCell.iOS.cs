using ObjCRuntime;

namespace Buform;

public abstract class PresentedPickerFormCell<TFormItem> : PickerFormCell<TFormItem>
    where TFormItem : class, IOptionsPickerFormItem
{
    protected virtual PickerPresenter<TFormItem>? PickerPresenter { get; set; }

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected PresentedPickerFormCell()
    {
        /* Required constructor */
    }

    protected PresentedPickerFormCell(NativeHandle handle)
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

    protected virtual PickerPresenter<TFormItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.Default => new DefaultPickerPresenter<TFormItem>(CreateViewController),
            PickerInputType.Dialog => new DialogPickerPresenter<TFormItem>(CreateViewController),
            _ => default,
        };
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel(Item?.Label);
        UpdateInputType();
        UpdateValue(Item?.FormattedValue);
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
