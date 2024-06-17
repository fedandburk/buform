using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public class TextFormCell<TItem> : FormCell<TItem>
    where TItem : class, ITextFormItem
{
    protected virtual UILabel? Label { get; set; }

    public TextFormCell()
    {
        /* Required constructor */
    }

    public TextFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        SelectionStyle = UITableViewCellSelectionStyle.Default;

        Label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = UIFont.PreferredBody,
            TextColor = UIColor.Label
        };

        ContentView.AddSubviews(Label);

        ContentView.AddConstraints(
            [
                Label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor),
                Label.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                )
            ]
        );
    }

    protected virtual void UpdateValue()
    {
        if (Label == null)
        {
            return;
        }

        Label.Text = Item?.FormattedValue;
    }

    protected override void OnItemSet()
    {
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Item.FormattedValue):
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
        }

        base.Dispose(disposing);
    }
}

[Preserve(AllMembers = true)]
[Register(nameof(TextFormCell))]
public sealed class TextFormCell : TextFormCell<ITextFormItem>
{
    public TextFormCell()
    {
        /* Required constructor */
    }

    public TextFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
