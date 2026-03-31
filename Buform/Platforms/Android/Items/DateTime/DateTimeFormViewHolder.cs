using System.Globalization;
using Android.Runtime;
using Android.Views;
using Google.Android.Material.DatePicker;
using Google.Android.Material.TextView;
using Java.Lang;
using Object = Java.Lang.Object;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class DateTimeFormViewHolder : FormViewHolder<DateTimeFormItem>
{
    private MaterialTextView? _labelView;
    private MaterialTextView? _valueView;

    public DateTimeFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public DateTimeFormViewHolder(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        _labelView = ItemView.FindViewById<MaterialTextView>(Resource.Id.Label);
        _valueView = ItemView.FindViewById<MaterialTextView>(Resource.Id.Value);

        ItemView.Click += OnClick;
    }

    protected override void OnDataSet()
    {
        UpdateLabel();
        UpdateValue();
        UpdateReadOnlyState();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(DateTimeFormItem.Label):
                UpdateLabel();
                break;
            case nameof(DateTimeFormItem.Value):
                UpdateValue();
                break;
            case nameof(DateTimeFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            default:
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ItemView.Click -= OnClick;
        }

        base.Dispose(disposing);
    }

    private void UpdateLabel()
    {
        if (_labelView == null)
        {
            return;
        }

        var label = Data?.Label;

        if (string.IsNullOrWhiteSpace(label))
        {
            _labelView.Text = string.Empty;
            _labelView.Visibility = ViewStates.Gone;
            return;
        }

        _labelView.Text = label;
        _labelView.Visibility = ViewStates.Visible;
    }

    private void UpdateValue()
    {
        if (_valueView == null)
        {
            return;
        }

        var text = GetDisplayValue();
        _valueView.Text = text;
    }

    private void UpdateReadOnlyState()
    {
        var isReadOnly = Data?.IsReadOnly ?? true;

        ItemView.Enabled = !isReadOnly;
        ItemView.Clickable = !isReadOnly;
        ItemView.Focusable = !isReadOnly;

        if (_valueView != null)
        {
            _valueView.Enabled = !isReadOnly;
            _valueView.Alpha = isReadOnly ? 0.6f : 1f;
        }

        if (_labelView != null)
        {
            _labelView.Enabled = !isReadOnly;
            _labelView.Alpha = isReadOnly ? 0.6f : 1f;
        }
    }

    private string GetDisplayValue()
    {
        if (Data?.Value == null)
        {
            return string.Empty;
        }

        return Data.InputType switch
        {
            DateTimeInputType.Date => Data.Value.Value.ToString("d", CultureInfo.CurrentCulture),
            DateTimeInputType.Time => Data.Value.Value.ToString("t", CultureInfo.CurrentCulture),
            _ => Data.Value.Value.ToString("g", CultureInfo.CurrentCulture)
        };
    }

    private void OnClick(object? sender, EventArgs e)
    {
        if (Data == null || Data.IsReadOnly)
        {
            return;
        }

        ShowDatePicker();
    }

    private void ShowDatePicker()
    {
        var activity = ItemView.Context?.GetFragmentActivity();
        if (activity == null)
        {
            return;
        }

        var currentValue = GetEffectiveValue();
        var selection = ToUtcMillis(currentValue);

        var constraintsBuilder = new CalendarConstraints.Builder();

        if (Data != null)
        {
            constraintsBuilder.SetStart(ToUtcMillis(Data.MinValue.Date));
            constraintsBuilder.SetEnd(ToUtcMillis(Data.MaxValue.Date));
        }

        var picker = MaterialDatePicker
            .Builder.DatePicker()
            .SetSelection(selection)
            .SetCalendarConstraints(constraintsBuilder.Build())
            .Build();

        picker.AddOnPositiveButtonClickListener(
            new DateSelectedListener(selectionValue =>
            {
                if (selectionValue is not Long selectedMillis || Data == null)
                {
                    return;
                }

                var selectedDate = FromUtcMillis(selectedMillis.LongValue()).Date;
                var current = GetEffectiveValue();

                var newValue = new DateTime(
                    selectedDate.Year,
                    selectedDate.Month,
                    selectedDate.Day,
                    current.Hour,
                    current.Minute,
                    current.Second,
                    current.Kind
                );

                SetClampedValue(newValue);
            })
        );

        picker.Show(activity.SupportFragmentManager, "DATE_PICKER");
    }

    private DateTime GetEffectiveValue()
    {
        if (Data?.Value != null)
        {
            return Data.Value.Value;
        }

        var now = DateTime.Now;

        if (Data == null)
        {
            return now;
        }

        if (Data.MinValue != default && now < Data.MinValue)
        {
            return Data.MinValue;
        }

        if (Data.MaxValue != default && now > Data.MaxValue)
        {
            return Data.MaxValue;
        }

        return now;
    }

    private void SetClampedValue(DateTime value)
    {
        if (Data == null)
        {
            return;
        }

        if (value < Data.MinValue)
        {
            value = Data.MinValue;
        }

        if (value > Data.MaxValue)
        {
            value = Data.MaxValue;
        }

        Data.Value = value;
    }

    private static long ToUtcMillis(DateTime dateTime)
    {
        var utc = dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime()
        };

        return new DateTimeOffset(utc).ToUnixTimeMilliseconds();
    }

    private static DateTime FromUtcMillis(long millis)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(millis).LocalDateTime;
    }

    private sealed class DateSelectedListener : Object, IMaterialPickerOnPositiveButtonClickListener
    {
        private readonly Action<Object?> _onSelected;

        public DateSelectedListener(Action<Object?> onSelected)
        {
            _onSelected = onSelected;
        }

        public void OnPositiveButtonClick(Object? selection)
        {
            _onSelected(selection);
        }
    }
}
