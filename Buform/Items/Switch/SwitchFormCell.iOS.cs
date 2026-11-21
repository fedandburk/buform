using ObjCRuntime;

namespace Buform;

public abstract class SwitchFormCell<TItem> : FormCell<TItem>
    where TItem : SwitchFormItem
{
    protected SwitchFormCell()
    {
        /* Required constructor */
    }

    protected SwitchFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;

    protected virtual UISwitch? Switch { get; set; }

    protected override void Initialize()
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        Label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = FormFonts.GetBody(),
            TextColor = UIColor.Label,
        };

        Switch = new UISwitch();
        Switch.ValueChanged += OnValueChanged;

        AccessoryView = Switch;

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
        Switch?.Enabled = !Item?.IsReadOnly ?? true;
    }

    protected virtual void UpdateLabel()
    {
        Label?.Text = Item?.Label;
    }

    protected virtual void UpdateValue(bool isAnimated)
    {
        Switch?.SetState(Item?.Value ?? false, isAnimated);
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateValue(false);
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Item.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(Item.Label):
                UpdateLabel();
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
            var @switch = Switch;
            if (@switch != null)
            {
                @switch.ValueChanged -= OnValueChanged;
            }

            Switch?.Dispose();
            Switch = null;

            Label?.Dispose();
            Label = null;
        }

        base.Dispose(disposing);
    }

    private void OnValueChanged(object? sender, EventArgs e)
    {
        Item?.Value = Switch?.On ?? false;
    }
}

[Register(nameof(SwitchFormCell))]
public sealed class SwitchFormCell : SwitchFormCell<SwitchFormItem>
{
    public SwitchFormCell()
    {
        /* Required constructor */
    }

    public SwitchFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
