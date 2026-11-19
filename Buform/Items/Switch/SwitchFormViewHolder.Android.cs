using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class SwitchFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : SwitchFormItem
{
    private bool _isUpdatingValue;

    protected virtual TextView? LabelView { get; set; }

    protected virtual CompoundButton? SwitchView { get; set; }

    protected SwitchFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected SwitchFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnItemViewClick(object? sender, EventArgs e)
    {
        SwitchView?.Toggle();
    }

    private void OnCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        if (_isUpdatingValue || Item == null)
        {
            return;
        }

        Item.Value = e.IsChecked;
    }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        SwitchView = ItemView.FindViewById<CompoundButton>(Resource.Id.Switch);
        ValidationErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        ItemView.Click += OnItemViewClick;

        SwitchView?.CheckedChange += OnCheckedChange;
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isEnabled = !(Item?.IsReadOnly ?? true);

        ItemView.Enabled = isEnabled;

        LabelView?.Enabled = isEnabled;
        SwitchView?.Enabled = isEnabled;
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Item?.Label;
    }

    protected virtual void UpdateValue()
    {
        if (SwitchView == null || Item == null)
        {
            return;
        }

        _isUpdatingValue = true;

        try
        {
            SwitchView.Checked = Item.Value;
        }
        finally
        {
            _isUpdatingValue = false;
        }
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
            case nameof(SwitchFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(SwitchFormItem.Label):
                UpdateLabel();
                break;
            case nameof(SwitchFormItem.Value):
                UpdateValue();
                break;
            case null:
                UpdateReadOnlyState();
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

            SwitchView?.CheckedChange -= OnCheckedChange;

            LabelView = null;
            SwitchView = null;
        }

        base.Dispose(disposing);
    }
}

public sealed class SwitchFormViewHolder : SwitchFormViewHolder<SwitchFormItem>
{
    public SwitchFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public SwitchFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
