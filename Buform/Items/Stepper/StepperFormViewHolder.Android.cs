using System.Globalization;
using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class StepperFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : StepperFormItem
{
    private const int DecreaseDirection = -1;
    private const int IncreaseDirection = 1;

    protected StepperFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected StepperFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected virtual TextView? LabelView { get; set; }

    protected virtual TextView? ValueView { get; set; }

    protected virtual View? DecreaseButtonView { get; set; }

    protected virtual View? IncreaseButtonView { get; set; }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        ValueView = ItemView.FindViewById<TextView>(Resource.Id.Value);
        DecreaseButtonView = ItemView.FindViewById(Resource.Id.DecreaseButton);
        IncreaseButtonView = ItemView.FindViewById(Resource.Id.IncreaseButton);
        ValidationErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        DecreaseButtonView?.ContentDescription = FormTexts.Decrease;
        IncreaseButtonView?.ContentDescription = FormTexts.Increase;

        DecreaseButtonView?.Click += OnDecreaseClick;
        IncreaseButtonView?.Click += OnIncreaseClick;
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isEnabled = !(Item?.IsReadOnly ?? true);

        LabelView?.Enabled = isEnabled;
        ValueView?.Enabled = isEnabled;

        UpdateButtonState();
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Item?.Label;
    }

    protected virtual void UpdateRange()
    {
        UpdateButtonState();
    }

    protected virtual void UpdateValue()
    {
        ValueView?.Text = Item?.Value.ToString(CultureInfo.CurrentCulture);

        UpdateButtonState();
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateRange();
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(StepperFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(StepperFormItem.Label):
                UpdateLabel();
                break;
            case nameof(StepperFormItem.MinValue):
            case nameof(StepperFormItem.MaxValue):
                UpdateRange();
                break;
            case nameof(StepperFormItem.Value):
                UpdateValue();
                break;
            case null:
                UpdateReadOnlyState();
                UpdateLabel();
                UpdateRange();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DecreaseButtonView?.Click -= OnDecreaseClick;
            IncreaseButtonView?.Click -= OnIncreaseClick;

            LabelView = null;
            ValueView = null;
            DecreaseButtonView = null;
            IncreaseButtonView = null;
        }

        base.Dispose(disposing);
    }

    private void OnDecreaseClick(object? sender, EventArgs e)
    {
        ChangeValue(DecreaseDirection);
    }

    private void OnIncreaseClick(object? sender, EventArgs e)
    {
        ChangeValue(IncreaseDirection);
    }

    private void ChangeValue(int direction)
    {
        if (Item == null || Item.IsReadOnly)
        {
            return;
        }

        var value = (long)Item.Value + (long)direction * Item.Step;
        var clampedValue = Math.Clamp(value, Item.MinValue, Item.MaxValue);

        Item.Value = (int)clampedValue;
    }

    private void UpdateButtonState()
    {
        if (Item == null)
        {
            return;
        }

        var isEnabled = !Item.IsReadOnly;

        DecreaseButtonView?.Enabled = isEnabled && Item.Value > Item.MinValue;
        IncreaseButtonView?.Enabled = isEnabled && Item.Value < Item.MaxValue;
    }
}

public sealed class StepperFormViewHolder : StepperFormViewHolder<StepperFormItem>
{
    public StepperFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public StepperFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
