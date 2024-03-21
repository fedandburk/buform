using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class CallbackPickerFormCellBase<TCallbackPickerItem> : PickerFormCellBase<TCallbackPickerItem> where TCallbackPickerItem : class, ICallbackPickerFormItem
{
    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected CallbackPickerFormCellBase()
    {
        /* Required constructor */
    }

    protected CallbackPickerFormCellBase(NativeHandle handle)
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
