using ObjCRuntime;

namespace Buform;

public abstract class PickerFormCell<TItem> : FormCell<TItem>
    where TItem : class, IFormItem
{
    protected virtual UILabel? Label { get; set; }

    protected virtual UILabel? ValueLabel { get; set; }

    protected override UILabel? ValidationLabel => Label;

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected PickerFormCell()
    {
        /* Required constructor */
    }

    protected PickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected abstract string? GetLabel(TItem item);

    protected abstract string? GetValue(TItem item);

    protected override void Initialize()
    {
        Label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = FormFonts.GetBody(),
            TextColor = UIColor.Label,
        };

        ValueLabel = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = FormFonts.GetBody(),
            TextColor = UIColor.SecondaryLabel,
            TextAlignment = UITextAlignment.Right,
        };

        ContentView.AddSubviews(Label, ValueLabel);

        ContentView.AddConstraints(
            new[]
            {
                Label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor),
                ValueLabel.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                ValueLabel.BottomAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.BottomAnchor
                ),
                ValueLabel.LeadingAnchor.ConstraintEqualTo(Label.TrailingAnchor, 10),
                ValueLabel.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                ),
            }
        );
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isReadOnly = Item?.IsReadOnly ?? true;

        Label?.Enabled = !isReadOnly;
        ValueLabel?.Enabled = !isReadOnly;

        Accessory = isReadOnly
            ? UITableViewCellAccessory.None
            : UITableViewCellAccessory.DisclosureIndicator;

        SelectionStyle = isReadOnly
            ? UITableViewCellSelectionStyle.None
            : UITableViewCellSelectionStyle.Default;
    }

    protected virtual void UpdateLabel()
    {
        Label?.Text = Item == null ? null : GetLabel(Item);
    }

    protected virtual void UpdateValue()
    {
        ValueLabel?.Text = Item == null ? null : GetValue(Item);
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
            case nameof(IFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case null:
                OnItemSet();
                break;
            default:
                UpdateLabel();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Label?.Dispose();
            Label = null;

            ValueLabel?.Dispose();
            ValueLabel = null;
        }

        base.Dispose(disposing);
    }
}
