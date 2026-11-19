using ObjCRuntime;

namespace Buform;

public abstract class SwitchFormCell<TItem> : FormCell<TItem>
    where TItem : SwitchFormItem
{
    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;
    protected virtual UISwitch? Switch { get; set; }

    protected SwitchFormCell()
    {
        /* Required constructor */
    }

    protected SwitchFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

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

    private void OnValueChanged(object? sender, EventArgs e)
    {
        if (Item == null)
        {
            return;
        }

        Item.Value = Switch?.On ?? false;
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (Switch == null)
        {
            return;
        }

        Switch.Enabled = !Item?.IsReadOnly ?? true;
    }

    protected virtual void UpdateLabel()
    {
        if (Label == null)
        {
            return;
        }

        Label.Text = Item?.Label;
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
