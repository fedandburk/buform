namespace Buform;

public abstract class PickerFormCell<TItem> : FormCell<TItem>
    where TItem : class, IFormItem
{
    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;
    protected virtual UILabel? ValueLabel { get; set; }

    protected PickerFormCell()
    {
        /* Required constructor */
    }

    protected PickerFormCell(IntPtr handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        Accessory = UITableViewCellAccessory.DisclosureIndicator;

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
        if (Label == null)
        {
            return;
        }

        SelectionStyle =
            Item?.IsReadOnly ?? true
                ? UITableViewCellSelectionStyle.None
                : UITableViewCellSelectionStyle.Default;
    }

    protected virtual void UpdateLabel(string? text)
    {
        if (Label == null)
        {
            return;
        }

        Label.Text = text;
    }

    protected virtual void UpdateValue(string? text)
    {
        if (ValueLabel == null)
        {
            return;
        }

        ValueLabel.Text = text;
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
