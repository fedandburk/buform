using ObjCRuntime;

namespace Buform;

public abstract class DateTimeFormCell<TItem> : FormCell<TItem>
    where TItem : DateTimeFormItem
{
    protected virtual UILabel? Label { get; set; }

    protected override UILabel? ValidationLabel => Label;
    protected virtual UIDatePicker? DatePicker { get; set; }

    protected DateTimeFormCell()
    {
        /* Required constructor */
    }

    protected DateTimeFormCell(NativeHandle handle)
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

        DatePicker = new UIDatePicker { TranslatesAutoresizingMaskIntoConstraints = false };

        DatePicker.ValueChanged += OnValueChanged;

        DatePicker.SetContentCompressionResistancePriority(1000, UILayoutConstraintAxis.Horizontal);

        ContentView.AddSubviews(Label, DatePicker);

        ContentView.AddConstraints(
            new[]
            {
                Label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor),
                DatePicker.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 4),
                DatePicker.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -4),
                DatePicker.LeadingAnchor.ConstraintEqualTo(Label.TrailingAnchor, 10),
                DatePicker.TrailingAnchor.ConstraintEqualTo(
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

        Item.Value = DatePicker?.Date.ToDateTime() ?? DateTime.UtcNow;
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (DatePicker == null)
        {
            return;
        }

        DatePicker.Enabled = !Item?.IsReadOnly ?? false;
    }

    protected virtual void UpdateRange()
    {
        if (DatePicker == null)
        {
            return;
        }

        DatePicker.MinimumDate = Item?.MinValue.ToDate() ?? NSDate.DistantPast;
        DatePicker.MaximumDate = Item?.MaxValue.ToDate() ?? NSDate.DistantFuture;
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
        if (DatePicker == null)
        {
            return;
        }

        var inputType = Item?.InputType;

        switch (inputType)
        {
            case DateTimeInputType.Date:
                DatePicker.Mode = UIDatePickerMode.Date;
                break;
            case DateTimeInputType.Time:
                DatePicker.Mode = UIDatePickerMode.Time;
                break;
            case DateTimeInputType.DateTime:
            case null:
                DatePicker.Mode = UIDatePickerMode.DateAndTime;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null);
        }
    }

    protected virtual void UpdateValue()
    {
        if (DatePicker == null)
        {
            return;
        }

        var dateTime = Item?.Value ?? DateTime.UtcNow;
        var date = dateTime.ToDate();

        if (DatePicker.MaximumDate?.Compare(date) == NSComparisonResult.Ascending)
        {
            date = DatePicker.MaximumDate;
        }

        if (DatePicker.MinimumDate?.Compare(date) == NSComparisonResult.Descending)
        {
            date = DatePicker.MinimumDate;
        }

        DatePicker.Date = date;
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateRange();
        UpdateLabel();
        UpdateInputType();
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Item.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(Item.MinValue):
            case nameof(Item.MaxValue):
                UpdateRange();
                break;
            case nameof(Item.Label):
                UpdateLabel();
                break;
            case nameof(Item.InputType):
                UpdateInputType();
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
            var datePicker = DatePicker;
            if (datePicker != null)
            {
                datePicker.ValueChanged -= OnValueChanged;
            }

            DatePicker?.Dispose();
            DatePicker = null;

            Label?.Dispose();
            Label = null;
        }

        base.Dispose(disposing);
    }
}

[Register(nameof(DateTimeFormCell))]
public sealed class DateTimeFormCell : DateTimeFormCell<DateTimeFormItem>
{
    public DateTimeFormCell()
    {
        /* Required constructor */
    }

    public DateTimeFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
