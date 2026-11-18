using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class CallbackPickerFormCell<TItem> : PickerFormCellBase<TItem>
    where TItem : class, ICallbackPickerFormItem
{
    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected CallbackPickerFormCell()
    {
        /* Required constructor */
    }

    protected CallbackPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel(Item?.Label);
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
            case nameof(Item.Value):
                UpdateValue(Item?.FormattedValue);
                break;
            case nameof(Item.ValidationErrorMessage):
                UpdateValidationErrorMessage(Item?.ValidationErrorMessage);
                break;
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();

        Item?.ExecuteCallback();
    }
}

[Preserve(AllMembers = true)]
[Register(nameof(CallbackPickerFormCell))]
public sealed class CallbackPickerFormCell : CallbackPickerFormCell<ICallbackPickerFormItem>
{
    public CallbackPickerFormCell()
    {
        /* Required constructor */
    }

    public CallbackPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
