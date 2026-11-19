using System.Globalization;
using Android.Runtime;
using Android.Views;

namespace Buform;

public class StepperFormViewHolder : FormViewHolder<StepperFormItem>
{
    protected virtual TextView? LabelView { get; set; }

    protected virtual TextView? ValueView { get; set; }

    protected virtual View? DecreaseButton { get; set; }

    protected virtual View? IncreaseButton { get; set; }

    protected virtual TextView? ErrorView { get; set; }

    protected override TextView? ValidationErrorView => ErrorView;

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

    private void OnDecreaseClick(object? sender, EventArgs e)
    {
        Step(-1);
    }

    private void OnIncreaseClick(object? sender, EventArgs e)
    {
        Step(1);
    }

    private void Step(int direction)
    {
        if (Data == null || Data.IsReadOnly)
        {
            return;
        }

        var value = (long)Data.Value + (long)direction * Data.StepAmount;
        var clampedValue = Math.Clamp(value, Data.MinValue, Data.MaxValue);

        Data.Value = (int)clampedValue;
    }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        ValueView = ItemView.FindViewById<TextView>(Resource.Id.Value);
        DecreaseButton = ItemView.FindViewById(Resource.Id.DecreaseButton);
        IncreaseButton = ItemView.FindViewById(Resource.Id.IncreaseButton);
        ErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        DecreaseButton?.ContentDescription = FormTexts.Decrease;
        IncreaseButton?.ContentDescription = FormTexts.Increase;

        DecreaseButton?.Click += OnDecreaseClick;
        IncreaseButton?.Click += OnIncreaseClick;
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Data?.Label;
    }

    protected virtual void UpdateValue()
    {
        if (Data == null)
        {
            return;
        }

        var isEnabled = !Data.IsReadOnly;

        ValueView?.Text = Data.Value.ToString(CultureInfo.CurrentCulture);

        LabelView?.Enabled = isEnabled;
        ValueView?.Enabled = isEnabled;

        DecreaseButton?.Enabled = isEnabled && Data.Value > Data.MinValue;
        IncreaseButton?.Enabled = isEnabled && Data.Value < Data.MaxValue;
    }

    protected override void OnDataSet()
    {
        UpdateLabel();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(StepperFormItem.Label):
                UpdateLabel();
                break;
            case nameof(StepperFormItem.IsReadOnly):
            case nameof(StepperFormItem.Value):
            case nameof(StepperFormItem.MinValue):
            case nameof(StepperFormItem.MaxValue):
                UpdateValue();
                break;
            case null:
                UpdateLabel();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DecreaseButton?.Click -= OnDecreaseClick;
            IncreaseButton?.Click -= OnIncreaseClick;

            LabelView = null;
            ValueView = null;
            DecreaseButton = null;
            IncreaseButton = null;
            ErrorView = null;
        }

        base.Dispose(disposing);
    }
}
