using ObjCRuntime;

namespace Buform;

public abstract class SegmentsFormCell<TItem> : FormCell<TItem>
    where TItem : class, ISegmentsFormItem
{
    private NSLayoutConstraint? _segmentedControlLeadingConstraint;
    private NSLayoutConstraint? _labelWidthConstraint;

    private IReadOnlyList<IFormOption> _options;

    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;
    protected virtual UISegmentedControl? SegmentedControl { get; set; }

    protected SegmentsFormCell()
    {
        _options = [];
    }

    protected SegmentsFormCell(NativeHandle handle)
        : base(handle)
    {
        _options = [];
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

        SegmentedControl = new UISegmentedControl
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        SegmentedControl.ValueChanged += OnValueChanged;

        SegmentedControl.SetContentCompressionResistancePriority(
            1000,
            UILayoutConstraintAxis.Horizontal
        );

        ContentView.AddSubviews(Label, SegmentedControl);

        _segmentedControlLeadingConstraint = SegmentedControl.LeadingAnchor.ConstraintEqualTo(
            Label.TrailingAnchor,
            10
        );

        _labelWidthConstraint = Label.WidthAnchor.ConstraintEqualTo(0);

        ContentView.AddConstraints(
            new[]
            {
                Label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor),
                _labelWidthConstraint,
                SegmentedControl.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 8),
                SegmentedControl.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -8),
                _segmentedControlLeadingConstraint,
                SegmentedControl.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                ),
            }
        );
    }

    private void OnValueChanged(object? sender, EventArgs e)
    {
        if (SegmentedControl == null)
        {
            return;
        }

        if (Item == null)
        {
            return;
        }

        if (_options.Count == 0)
        {
            Item.Value = null;
        }
        else if (SegmentedControl.SelectedSegment < 0)
        {
            Item.Value = null;
        }
        else
        {
            Item.Value = _options.ElementAtOrDefault((int)SegmentedControl.SelectedSegment)?.Value;
        }
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (SegmentedControl == null)
        {
            return;
        }

        SegmentedControl.Enabled = !Item?.IsReadOnly ?? true;
    }

    protected virtual void UpdateLabel()
    {
        if (Label == null)
        {
            return;
        }

        Label.Text = Item?.Label;

        if (_segmentedControlLeadingConstraint == null || _labelWidthConstraint == null)
        {
            return;
        }

        var hasLabel = !string.IsNullOrWhiteSpace(Item?.Label);

        _segmentedControlLeadingConstraint.Constant = hasLabel ? 10 : 0;
        _labelWidthConstraint.Active = !hasLabel;
    }

    protected virtual void UpdateOptions()
    {
        if (SegmentedControl == null)
        {
            return;
        }

        _options = Item?.Options ?? [];

        SegmentedControl.RemoveAllSegments();

        for (var index = 0; index < _options.Count; index++)
        {
            var value = _options[index];

            SegmentedControl.InsertSegment(value.FormattedValue ?? string.Empty, index, false);
        }
    }

    protected virtual void UpdateValue()
    {
        if (SegmentedControl == null)
        {
            return;
        }

        if (_options.Count == 0 || Item?.Value == null)
        {
            SegmentedControl.SelectedSegment = nint.MinValue;
        }
        else
        {
            var index = _options.ToList().FindIndex(option => Equals(option.Value, Item.Value));

            SegmentedControl.SelectedSegment = index;
        }
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateOptions();
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
            case nameof(Item.Options):
                UpdateOptions();
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
            var segmentedControl = SegmentedControl;
            if (segmentedControl != null)
            {
                segmentedControl.ValueChanged -= OnValueChanged;
            }

            SegmentedControl?.Dispose();
            SegmentedControl = null;

            Label?.Dispose();
            Label = null;
        }

        base.Dispose(disposing);
    }
}

[Register(nameof(SegmentsFormCell))]
public sealed class SegmentsFormCell : SegmentsFormCell<ISegmentsFormItem>
{
    public SegmentsFormCell()
    {
        /* Required constructor */
    }

    public SegmentsFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
