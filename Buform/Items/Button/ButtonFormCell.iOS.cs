using ObjCRuntime;

namespace Buform;

public abstract class ButtonFormCell<TItem> : FormCell<TItem>
    where TItem : ButtonFormItem
{
    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected virtual UILabel? Label { get; set; }

    public ButtonFormCell(NSString reuseIdentifier)
        : base(UITableViewCellStyle.Default, reuseIdentifier)
    {
        /* Required constructor */
    }

    public ButtonFormCell()
    {
        /* Required constructor */
    }

    public ButtonFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
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
        if (Label == null)
        {
            return;
        }

        Label.Text = Item?.Label;
    }

    protected virtual void UpdateInputType()
    {
        if (Label == null)
        {
            return;
        }

        var inputType = Item?.InputType;

        Label.TextColor = inputType switch
        {
            ButtonInputType.Default => TintColor,
            ButtonInputType.Done => TintColor,
            ButtonInputType.Destructive => UIColor.SystemRed,
            null => TintColor,
            _ => throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null),
        };

        Label.Font =
            inputType == ButtonInputType.Done ? FormFonts.GetBoldBody() : FormFonts.GetBody();
    }

    public override void TintColorDidChange()
    {
        base.TintColorDidChange();

        UpdateInputType();
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateInputType();
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
            case nameof(Item.InputType):
                UpdateInputType();
                break;
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();

        Item?.Value.SafeExecute();
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
    public ButtonFormCell(NSString reuseIdentifier)
        : base(reuseIdentifier)
    {
        /* Required constructor */
    }

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
