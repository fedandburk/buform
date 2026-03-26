using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class CallbackPickerFormViewHolder<TItem> : PickerFormViewHolderBase<TItem>
    where TItem : class, ICallbackPickerFormItem
{
    public virtual bool IsSelectable => !(Data?.IsReadOnly ?? true);

    protected CallbackPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    protected CallbackPickerFormViewHolder(View itemView)
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
        if (!IsSelectable)
        {
            return;
        }

        Data?.ExecuteCallback();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel(Data?.Label);
        UpdateValue(Data?.FormattedValue);
        UpdateValidationErrorMessage(Data?.ValidationErrorMessage);
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ICallbackPickerFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ICallbackPickerFormItem.Label):
                UpdateLabel(Data?.Label);
                break;
            case nameof(ICallbackPickerFormItem.FormattedValue):
                UpdateValue(Data?.FormattedValue);
                break;
            case nameof(ICallbackPickerFormItem.ValidationErrorMessage):
                UpdateValidationErrorMessage(Data?.ValidationErrorMessage);
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachListeners();
        }

        base.Dispose(disposing);
    }
}

[Preserve(AllMembers = true)]
[Register(nameof(CallbackPickerFormViewHolder))]
public sealed class CallbackPickerFormViewHolder
    : CallbackPickerFormViewHolder<ICallbackPickerFormItem>
{
    public CallbackPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public CallbackPickerFormViewHolder(View itemView)
        : base(itemView) { }
}
