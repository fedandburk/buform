using Android.Runtime;
using Android.Views;
using Google.Android.Material.Slider;
using Object = Java.Lang.Object;

namespace Buform;

public class SliderFormViewHolder : FormViewHolder<SliderFormItem>
{
    private ChangeListener? _changeListener;

    protected virtual Slider? SliderView { get; set; }

    public SliderFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public SliderFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnValueChange(float value, bool isFromUser)
    {
        if (!isFromUser || Data == null)
        {
            return;
        }

        Data.Value = value;
    }

    protected override void Initialize()
    {
        SliderView = ItemView.FindViewById<Slider>(Resource.Id.Slider);

        _changeListener = new ChangeListener(this);

        SliderView?.AddOnChangeListener(_changeListener);
    }

    protected virtual void UpdateReadOnlyState()
    {
        SliderView?.Enabled = !(Data?.IsReadOnly ?? true);
    }

    protected virtual void UpdateRange()
    {
        if (SliderView == null || Data == null)
        {
            return;
        }

        if (Data.MinValue >= Data.MaxValue)
        {
            throw new InvalidOperationException(
                $"{nameof(SliderFormItem)}.{nameof(SliderFormItem.MinValue)} must be less than {nameof(SliderFormItem.MaxValue)}."
            );
        }

        SliderView.ValueFrom = Data.MinValue;
        SliderView.ValueTo = Data.MaxValue;
    }

    protected virtual void UpdateValue()
    {
        if (SliderView == null || Data == null)
        {
            return;
        }

        SliderView.Value = Math.Clamp(Data.Value, Data.MinValue, Data.MaxValue);
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateRange();
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
            case null:
                UpdateReadOnlyState();
                UpdateRange();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_changeListener != null)
            {
                SliderView?.RemoveOnChangeListener(_changeListener);
            }

            _changeListener?.Dispose();
            _changeListener = null;

            SliderView = null;
        }

        base.Dispose(disposing);
    }

    private sealed class ChangeListener : Object, Slider.IOnChangeListener
    {
        private readonly SliderFormViewHolder _owner;

        public ChangeListener(SliderFormViewHolder owner)
        {
            _owner = owner;
        }

        public void OnValueChange(Slider slider, float value, bool fromUser)
        {
            _owner.OnValueChange(value, fromUser);
        }
    }
}
