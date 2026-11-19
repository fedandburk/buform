using Android.Runtime;
using Android.Views;
using Google.Android.Material.Slider;

namespace Buform;

public abstract class SliderFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : SliderFormItem
{
    private SliderChangeListener? _changeListener;

    protected virtual Slider? SliderView { get; set; }

    protected SliderFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected SliderFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnValueChange(float value, bool isFromUser)
    {
        if (!isFromUser || Item == null)
        {
            return;
        }

        Item.Value = value;
    }

    protected override void Initialize()
    {
        SliderView = ItemView.FindViewById<Slider>(Resource.Id.Slider);

        _changeListener = new SliderChangeListener(OnValueChange);

        SliderView?.AddOnChangeListener(_changeListener);
    }

    protected virtual void UpdateReadOnlyState()
    {
        SliderView?.Enabled = !(Item?.IsReadOnly ?? true);
    }

    protected virtual void UpdateRange()
    {
        if (SliderView == null || Item == null)
        {
            return;
        }

        if (Item.MinValue >= Item.MaxValue)
        {
            throw new InvalidOperationException(
                $"{nameof(SliderFormItem)}.{nameof(SliderFormItem.MinValue)} must be less than {nameof(SliderFormItem.MaxValue)}."
            );
        }

        SliderView.ValueFrom = Item.MinValue;
        SliderView.ValueTo = Item.MaxValue;
    }

    protected virtual void UpdateValue()
    {
        if (SliderView == null || Item == null)
        {
            return;
        }

        SliderView.Value = Math.Clamp(Item.Value, Item.MinValue, Item.MaxValue);
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateRange();
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
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
}

public sealed class SliderFormViewHolder : SliderFormViewHolder<SliderFormItem>
{
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
}
