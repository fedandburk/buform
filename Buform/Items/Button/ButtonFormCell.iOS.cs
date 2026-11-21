using ObjCRuntime;

namespace Buform;

public abstract class ButtonFormCell<TItem> : FormCell<TItem>
    where TItem : ButtonFormItem
{
    protected ButtonFormCell()
    {
        /* Required constructor */
    }

    protected ButtonFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected virtual UILabel? Label { get; set; }

    public override void TintColorDidChange()
    {
        base.TintColorDidChange();

        UpdateStyle();
    }

    public override void OnSelected()
    {
        base.OnSelected();

        Item?.Value.SafeExecute();
    }

    protected override void Initialize()
    {
        Label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = FormFonts.GetBody(),
            TextColor = UIColor.Label,
        };

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
        if (Label == null)
        {
            return;
        }

        var isReadOnly = Item?.IsReadOnly ?? true;

        Label.Enabled = !isReadOnly;

        SelectionStyle = isReadOnly
            ? UITableViewCellSelectionStyle.None
            : UITableViewCellSelectionStyle.Default;
    }

    protected virtual void UpdateLabel()
    {
        Label?.Text = Item?.Label;
    }

    protected virtual void UpdateStyle()
    {
        if (Label == null)
        {
            return;
        }

        var style = Item?.Style;

        Label.TextColor = style switch
        {
            ButtonStyle.Default => TintColor,
            ButtonStyle.Done => TintColor,
            ButtonStyle.Destructive => UIColor.SystemRed,
            null => TintColor,
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null),
        };

        Label.Font = style == ButtonStyle.Done ? FormFonts.GetBoldBody() : FormFonts.GetBody();
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateStyle();
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
            case nameof(Item.Style):
                UpdateStyle();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Label?.Dispose();
            Label = null;
        }

        base.Dispose(disposing);
    }
}

[Register(nameof(ButtonFormCell))]
public sealed class ButtonFormCell : ButtonFormCell<ButtonFormItem>
{
    public ButtonFormCell()
    {
        /* Required constructor */
    }

    public ButtonFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
