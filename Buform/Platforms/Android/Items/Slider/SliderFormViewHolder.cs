using Android.Runtime;
using Android.Views;
using Google.Android.Material.Slider;

namespace Buform;

[Preserve(AllMembers = true)]
public class SliderFormViewHolder : FormViewHolder<SliderFormItem>
{
    private const float Tolerance = 0.0001f;
    private bool _isUpdatingFromModel;
    private Slider? _slider;

    public SliderFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public SliderFormViewHolder(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        _slider = ItemView.FindViewById<Slider>(Resource.Id.Slider)!;

        _slider.Touch += OnSliderTouch;
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateRange();
        UpdateStepSize();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(SliderFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(SliderFormItem.MinValue):
            case nameof(SliderFormItem.MaxValue):
                UpdateRange();
                UpdateValue();
                break;
            case nameof(SliderFormItem.Value):
                UpdateValue();
                break;
            default:
                UpdateReadOnlyState();
                UpdateRange();
                UpdateStepSize();
                UpdateValue();
                break;
        }
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_slider == null)
        {
            return;
        }

        var isReadOnly = Data?.IsReadOnly ?? true;

        ItemView.Enabled = !isReadOnly;
        ItemView.Clickable = !isReadOnly;
        ItemView.Focusable = !isReadOnly;
        _slider.Enabled = !isReadOnly;
    }

    protected virtual void UpdateRange()
    {
        if (_slider == null || Data == null)
        {
            return;
        }

        var min = Data.MinValue;
        var max = Data.MaxValue;

        if (max < min)
        {
            max = min;
        }

        if (Math.Abs(_slider.ValueFrom - min) > Tolerance)
        {
            _slider.ValueFrom = min;
        }

        if (Math.Abs(_slider.ValueTo - max) > Tolerance)
        {
            _slider.ValueTo = max;
        }
    }

    protected virtual void UpdateStepSize()
    {
        if (_slider == null || Data == null)
        {
            return;
        }

        const int stepSize = 1;
        if (Math.Abs(_slider.StepSize - stepSize) > Tolerance)
        {
            _slider.StepSize = stepSize;
        }
    }

    protected virtual void UpdateValue()
    {
        if (_slider == null || Data == null)
        {
            return;
        }

        var value = Data.Value;
        value = Math.Clamp(value, _slider.ValueFrom, _slider.ValueTo);

        if (Math.Abs(_slider.Value - value) < Tolerance)
        {
            return;
        }

        _isUpdatingFromModel = true;
        try
        {
            _slider.Value = value;
        }
        finally
        {
            _isUpdatingFromModel = false;
        }
    }

    private void OnSliderTouch(object? sender, View.TouchEventArgs e)
    {
        e.Handled = false;

        if (_isUpdatingFromModel || Data == null || _slider == null)
        {
            return;
        }

        if (Math.Abs(Data.Value - _slider.Value) > Tolerance)
        {
            Data.Value = _slider.Value;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_slider != null)
            {
                _slider.Touch -= OnSliderTouch;
            }
            _slider = null;
        }

        base.Dispose(disposing);
    }
}
