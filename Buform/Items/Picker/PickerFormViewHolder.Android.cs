using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class PickerFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : class, IFormItem
{
    protected virtual TextView? LabelView { get; set; }

    protected virtual TextView? ValueView { get; set; }

    protected virtual TextView? ErrorView { get; set; }

    protected override TextView? ValidationErrorView => ErrorView;

    protected PickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected PickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnItemViewClick(object? sender, EventArgs e)
    {
        if (Data == null || Data.IsReadOnly)
        {
            return;
        }

        OnClick();
    }

    protected abstract string? GetLabel(TItem item);

    protected abstract string? GetValue(TItem item);

    protected abstract void OnClick();

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        ValueView = ItemView.FindViewById<TextView>(Resource.Id.Value);
        ErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        ItemView.Click += OnItemViewClick;
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isEnabled = !(Data?.IsReadOnly ?? true);

        ItemView.Enabled = isEnabled;

        LabelView?.Enabled = isEnabled;
        ValueView?.Enabled = isEnabled;
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Data == null ? null : GetLabel(Data);
    }

    protected virtual void UpdateValue()
    {
        ValueView?.Text = Data == null ? null : GetValue(Data);
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(IFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case null:
                UpdateReadOnlyState();
                UpdateLabel();
                UpdateValue();
                break;
            default:
                UpdateLabel();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ItemView.Click -= OnItemViewClick;

            LabelView = null;
            ValueView = null;
            ErrorView = null;
        }

        base.Dispose(disposing);
    }
}
