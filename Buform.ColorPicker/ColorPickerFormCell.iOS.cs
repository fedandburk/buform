using ObjCRuntime;

namespace Buform;

public abstract class ColorPickerFormCell<TItem> : FormCell<TItem>
    where TItem : ColorPickerFormItem
{
    protected ColorPickerFormCell()
    {
        /* Required constructor */
    }

    protected ColorPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected virtual IUIColorPickerViewControllerDelegate? Delegate { get; set; }

    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;

    protected virtual UIView? ColorView { get; set; }

    public override async void OnSelected()
    {
        base.OnSelected();

        if (Item == null)
        {
            return;
        }

        var colorPickerViewController = CreateColorPickerViewController(Item);

        var viewController = GetViewController();

        if (viewController == null)
        {
            return;
        }

        await viewController
            .PresentViewControllerAsync(colorPickerViewController, true)
            .ConfigureAwait(false);
    }

    protected virtual UIColorPickerViewController CreateColorPickerViewController(TItem item)
    {
        Delegate?.Dispose();
        Delegate = new ColorPickerViewControllerDelegate(item);

        return new UIColorPickerViewController
        {
            Delegate = Delegate,
            SelectedColor = item.Value.ToColor(),
        };
    }

    protected override void Initialize()
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        var bodyFont = UIFont.PreferredBody;

        if (bodyFont == null)
        {
            throw new InvalidOperationException("The system has no body font.");
        }

        Label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = bodyFont,
            TextColor = UIColor.Label,
        };

        ColorView = new UIView { Frame = new CGRect(0, 0, 22, 22), ClipsToBounds = true };

        ColorView.Layer.CornerRadius = 11;

        AccessoryView = ColorView;

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

    protected virtual void UpdateLabel()
    {
        Label?.Text = Item?.Label;
    }

    protected virtual void UpdateValue()
    {
        ColorView?.BackgroundColor = Item?.Value.ToColor() ?? UIColor.Clear;
    }

    protected virtual void UpdateReadOnlyState()
    {
        Label?.Enabled = !Item?.IsReadOnly ?? false;
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
            case nameof(Item.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(Item.Label):
                UpdateLabel();
                break;
            case nameof(Item.Value):
                UpdateValue();
                break;
            case null:
                OnItemSet();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ColorView?.Dispose();
            ColorView = null;

            Label?.Dispose();
            Label = null;

            Delegate?.Dispose();
            Delegate = null;
        }

        base.Dispose(disposing);
    }
}

[Register(nameof(ColorPickerFormCell))]
public sealed class ColorPickerFormCell : ColorPickerFormCell<ColorPickerFormItem>
{
    public ColorPickerFormCell()
    {
        /* Required constructor */
    }

    public ColorPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
