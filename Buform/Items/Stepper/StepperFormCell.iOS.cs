using ObjCRuntime;

namespace Buform;

public abstract class StepperFormCell<TItem> : FormCell<TItem>
    where TItem : StepperFormItem
{
    protected StepperFormCell()
    {
        /* Required constructor */
    }

    protected StepperFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;

    protected virtual UIStepper? Stepper { get; set; }

    protected override void Initialize()
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        Label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = FormFonts.GetBody(),
            TextColor = UIColor.Label,
        };

        Stepper = new UIStepper();
        Stepper.ValueChanged += OnValueChanged;

        AccessoryView = Stepper;

        ContentView.AddSubviews(Label);

        ContentView.AddConstraints(
            new[]
            {
                Label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor),
                Label.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                ),
            }
        );
    }

    protected virtual void UpdateReadOnlyState()
    {
        Stepper?.Enabled = !Item?.IsReadOnly ?? true;
    }

    protected virtual void UpdateRange()
    {
        if (Stepper == null)
        {
            return;
        }

        Stepper.MinimumValue = Item?.MinValue ?? float.MinValue;
        Stepper.MaximumValue = Item?.MaxValue ?? float.MaxValue;
    }

    protected virtual void UpdateStep()
    {
        Stepper?.StepValue = Item?.Step ?? 1;
    }

    protected virtual void UpdateLabel()
    {
        Label?.Text = Item?.Label;
    }

    protected virtual void UpdateValue()
    {
        Stepper?.Value = Item?.Value ?? 0;
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateRange();
        UpdateStep();
        UpdateLabel();
        UpdateValue();
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
            case nameof(Item.Step):
                UpdateStep();
                break;
            case nameof(Item.Label):
                UpdateLabel();
                break;
            case nameof(Item.Value):
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var stepper = Stepper;
            stepper?.ValueChanged -= OnValueChanged;

            Stepper?.Dispose();
            Stepper = null;

            Label?.Dispose();
            Label = null;
        }

        base.Dispose(disposing);
    }

    private void OnValueChanged(object? sender, EventArgs e)
    {
        Item?.Value = (int)(Stepper?.Value ?? 0);
    }
}

[Register(nameof(StepperFormCell))]
public sealed class StepperFormCell : StepperFormCell<StepperFormItem>
{
    public StepperFormCell()
    {
        /* Required constructor */
    }

    public StepperFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
