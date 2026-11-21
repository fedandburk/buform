using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class PickerFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : class, IFormItem
{
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

    protected virtual TextView? LabelView { get; set; }

    protected virtual TextView? ValueView { get; set; }

    protected abstract string? GetLabel(TItem item);

    protected abstract string? GetValue(TItem item);

    protected abstract Task OnClickAsync();

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        ValueView = ItemView.FindViewById<TextView>(Resource.Id.Value);
        ValidationErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        ItemView.Click += OnItemViewClick;
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isEnabled = !(Item?.IsReadOnly ?? true);

        ItemView.Enabled = isEnabled;

        LabelView?.Enabled = isEnabled;
        ValueView?.Enabled = isEnabled;
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Item == null ? null : GetLabel(Item);
    }

    protected virtual void UpdateValue()
    {
        ValueView?.Text = Item == null ? null : GetValue(Item);
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
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
        }

        base.Dispose(disposing);
    }

    private async void OnItemViewClick(object? sender, EventArgs e)
    {
        if (Item == null || Item.IsReadOnly)
        {
            return;
        }

        await OnClickAsync().ConfigureAwait(true);
    }
}
