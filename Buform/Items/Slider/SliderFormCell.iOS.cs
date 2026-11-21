using ObjCRuntime;

namespace Buform;

public abstract class SliderFormCell<TItem> : FormCell<TItem>
    where TItem : SliderFormItem
{
    protected SliderFormCell()
    {
        /* Required constructor */
    }

    protected SliderFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual UISlider? Slider { get; set; }

    protected override void Initialize()
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        Slider = new UISlider { TranslatesAutoresizingMaskIntoConstraints = false };

        Slider.ValueChanged += OnValueChanged;

        ContentView.AddSubviews(Slider);

        ContentView.AddConstraints(
            new[]
            {
                Slider.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Slider.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Slider.LeadingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.LeadingAnchor
                ),
                Slider.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                ),
            }
        );
    }

    protected virtual void UpdateReadOnlyState()
    {
        Slider?.Enabled = !Item?.IsReadOnly ?? true;
    }

    protected virtual void UpdateRange()
    {
        if (Slider == null)
        {
            return;
        }

        Slider.MinValue = Item?.MinValue ?? float.MinValue;
        Slider.MaxValue = Item?.MaxValue ?? float.MaxValue;
    }

    protected virtual void UpdateValue(bool isAnimated)
    {
        Slider?.SetValue(Item?.Value ?? 0f, isAnimated);
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateRange();
        UpdateValue(false);
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Item.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(Item.MinValue):
            case nameof(Item.MaxValue):
                UpdateRange();
                break;
            case nameof(Item.Value):
                UpdateValue(true);
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var slider = Slider;
            slider?.ValueChanged -= OnValueChanged;

            Slider?.Dispose();
            Slider = null;
        }

        base.Dispose(disposing);
    }

    private void OnValueChanged(object? sender, EventArgs e)
    {
        if (Slider == null)
        {
            return;
        }

        if (Item == null)
        {
            return;
        }

        Item.Value = Slider.Value;
    }
}

[Register(nameof(SliderFormCell))]
public sealed class SliderFormCell : SliderFormCell<SliderFormItem>
{
    public SliderFormCell()
    {
        /* Required constructor */
    }

    public SliderFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
