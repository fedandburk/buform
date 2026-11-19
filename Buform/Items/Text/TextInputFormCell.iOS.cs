using ObjCRuntime;

namespace Buform;

public abstract class TextInputFormCell<TItem> : FormCell<TItem>
    where TItem : class, ITextInputFormItem
{
    private NSLayoutConstraint? _textFieldLeadingConstraint;
    private NSLayoutConstraint? _labelWidthConstraint;

    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;
    protected virtual UITextField? TextField { get; set; }

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected TextInputFormCell()
    {
        /* Required constructor */
    }

    protected TextInputFormCell(NativeHandle handle)
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

        TextField = new UITextField
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = FormFonts.GetBody(),
            TextAlignment = UITextAlignment.Right,
            ClearButtonMode = UITextFieldViewMode.WhileEditing,
        };

        TextField.EditingChanged += OnEditingChanged;

        ContentView.AddSubviews(Label, TextField);

        _textFieldLeadingConstraint = Label.TrailingAnchor.ConstraintEqualTo(
            TextField.LeadingAnchor,
            -10
        );

        _labelWidthConstraint = Label.WidthAnchor.ConstraintEqualTo(0);

        ContentView.AddConstraints(
            new[]
            {
                Label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor),
                _labelWidthConstraint,
                TextField.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                TextField.BottomAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.BottomAnchor
                ),
                _textFieldLeadingConstraint,
                TextField.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                ),
            }
        );
    }

    private void OnEditingChanged(object? sender, EventArgs e)
    {
        if (TextField == null)
        {
            return;
        }

        Item?.SetText(TextField.Text);
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (TextField == null)
        {
            return;
        }

        TextField.Enabled = !Item?.IsReadOnly ?? true;
    }

    protected virtual void UpdateLabel()
    {
        if (Label == null)
        {
            return;
        }

        Label.Text = Item?.Label;

        if (_textFieldLeadingConstraint == null || _labelWidthConstraint == null)
        {
            return;
        }

        var hasLabel = !string.IsNullOrWhiteSpace(Item?.Label);

        _textFieldLeadingConstraint.Constant = hasLabel ? -10 : 0;
        _labelWidthConstraint.Active = !hasLabel;

        if (TextField == null)
        {
            return;
        }

        TextField.TextAlignment = hasLabel ? UITextAlignment.Right : UITextAlignment.Left;
    }

    protected virtual void UpdatePlaceholder()
    {
        if (TextField == null)
        {
            return;
        }

        TextField.Placeholder = Item?.Placeholder;
    }

    protected virtual void UpdateInputType()
    {
        if (TextField == null)
        {
            return;
        }

        TextField.KeyboardType = Item?.InputType switch
        {
            TextInputType.Default => UIKeyboardType.Default,
            TextInputType.NumberAndPunctuation => UIKeyboardType.NumbersAndPunctuation,
            TextInputType.Number => UIKeyboardType.NumberPad,
            TextInputType.Decimal => UIKeyboardType.DecimalPad,
            TextInputType.Phone => UIKeyboardType.PhonePad,
            TextInputType.Url => UIKeyboardType.Url,
            TextInputType.EmailAddress => UIKeyboardType.EmailAddress,
            null => UIKeyboardType.Default,
            _ => throw new ArgumentOutOfRangeException(
                nameof(Item.InputType),
                Item?.InputType,
                null
            ),
        };
    }

    protected virtual void UpdateSecureState()
    {
        if (TextField == null)
        {
            return;
        }

        TextField.SecureTextEntry = Item?.IsSecure ?? false;
    }

    protected virtual void UpdateValue()
    {
        if (TextField == null)
        {
            return;
        }

        TextField.Text = Item?.FormattedValue;
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdatePlaceholder();
        UpdateInputType();
        UpdateSecureState();
        UpdateValue();
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
            case nameof(Item.Placeholder):
                UpdatePlaceholder();
                break;
            case nameof(Item.InputType):
                UpdateInputType();
                break;
            case nameof(Item.IsSecure):
                UpdateSecureState();
                break;
            case nameof(Item.FormattedValue):
                UpdateValue();
                break;
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();

        TextField?.BecomeFirstResponder();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var textField = TextField;
            if (textField != null)
            {
                textField.EditingChanged -= OnEditingChanged;
            }

            TextField?.Dispose();
            TextField = null;

            Label?.Dispose();
            Label = null;
        }

        base.Dispose(disposing);
    }
}

[Register(nameof(TextInputFormCell))]
public sealed class TextInputFormCell : TextInputFormCell<ITextInputFormItem>
{
    public TextInputFormCell()
    {
        /* Required constructor */
    }

    public TextInputFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
