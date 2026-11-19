using ObjCRuntime;

namespace Buform;

public abstract class CallbackPickerFormCell<TItem> : PickerFormCell<TItem>
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
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();

        Item?.ExecuteCallback();
    }
}

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
