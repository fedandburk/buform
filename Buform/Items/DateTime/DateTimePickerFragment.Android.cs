using Android.Runtime;
using Android.Views;
using Google.Android.Material.DatePicker;
using Google.Android.Material.TimePicker;
using Java.Lang;
using Java.Util;
using DateFormat = Android.Text.Format.DateFormat;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using Object = Java.Lang.Object;

namespace Buform;

internal sealed class DateTimePickerFragment : Fragment
{
    private const string DatePickerTag = "DatePicker";
    private const string TimePickerTag = "TimePicker";

    private readonly DateTimeFormItem? _item;

    private DateTime _value;

    private DateTimeFormItem Item =>
        _item ?? throw new InvalidOperationException("The date and time picker has no item.");

    public DateTimePickerFragment()
    {
        /* Required constructor */
    }

    private DateTimePickerFragment(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public DateTimePickerFragment(DateTimeFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _item = item;
    }

    public static void Show(FragmentManager fragmentManager, DateTimeFormItem item)
    {
        ArgumentNullException.ThrowIfNull(fragmentManager);
        ArgumentNullException.ThrowIfNull(item);

        if (fragmentManager.FindFragmentByTag(nameof(DateTimePickerFragment)) != null)
        {
            return;
        }

        fragmentManager
            .BeginTransaction()
            .Add(new DateTimePickerFragment(item), nameof(DateTimePickerFragment))
            .CommitNow();
    }

    public override void OnCreate(Android.OS.Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (_item == null)
        {
            Close();

            return;
        }

        _value = _item.Value ?? Clamp(DateTime.Now);

        switch (_item.InputType)
        {
            case DateTimeInputType.Date:
            case DateTimeInputType.DateTime:
                ShowDatePicker();
                break;
            case DateTimeInputType.Time:
                ShowTimePicker();
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(DateTimeFormItem.InputType),
                    _item.InputType,
                    null
                );
        }
    }

    private void ShowDatePicker()
    {
        var validators = new List<CalendarConstraints.IDateValidator>
        {
            DateValidatorPointForward.From(ToSelection(Item.MinValue)),
            DateValidatorPointBackward.Before(ToSelection(Item.MaxValue)),
        };

        var constraints = new CalendarConstraints.Builder()
            .SetOpenAt(ToSelection(_value))
            .SetValidator(CompositeDateValidator.AllOf(validators))
            .Build();

        var picker = MaterialDatePicker
            .Builder.DatePicker()
            .SetTitleText(Item.Label)
            .SetSelection(Long.ValueOf(ToSelection(_value)))
            .SetCalendarConstraints(constraints)
            .Build();

        picker.AddOnPositiveButtonClickListener(new DateSelectedListener(this));
        picker.AddOnNegativeButtonClickListener(new CloseListener(this));
        picker.AddOnCancelListener(new CloseListener(this));

        picker.Show(ChildFragmentManager, DatePickerTag);
    }

    private void ShowTimePicker()
    {
        var timeFormat = DateFormat.Is24HourFormat(RequireContext())
            ? TimeFormat.Clock24h
            : TimeFormat.Clock12h;

        var picker = new MaterialTimePicker.Builder()
            .SetTimeFormat(timeFormat)
            .SetHour(_value.Hour)
            .SetMinute(_value.Minute)
            .SetTitleText(Item.Label)
            .Build();

        picker.AddOnPositiveButtonClickListener(new TimeSelectedListener(this, picker));
        picker.AddOnNegativeButtonClickListener(new CloseListener(this));
        picker.AddOnCancelListener(new CloseListener(this));

        picker.Show(ChildFragmentManager, TimePickerTag);
    }

    private void OnDateSelected(long selection)
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(selection).UtcDateTime;

        _value = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            _value.Hour,
            _value.Minute,
            _value.Second,
            _value.Kind
        );

        if (_item?.InputType == DateTimeInputType.DateTime)
        {
            ShowTimePicker();

            return;
        }

        Commit();
    }

    private void OnTimeSelected(int hour, int minute)
    {
        _value = new DateTime(_value.Year, _value.Month, _value.Day, hour, minute, 0, _value.Kind);

        Commit();
    }

    private void Commit()
    {
        _item?.Value = Clamp(_value);

        Close();
    }

    private void Close()
    {
        ParentFragmentManager.BeginTransaction().Remove(this).CommitAllowingStateLoss();
    }

    private DateTime Clamp(DateTime value)
    {
        if (value < Item.MinValue)
        {
            return Item.MinValue;
        }

        if (value > Item.MaxValue)
        {
            return Item.MaxValue;
        }

        return value;
    }

    private static long ToSelection(DateTime value)
    {
        var date = new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, TimeSpan.Zero);

        return date.ToUnixTimeMilliseconds();
    }

    private sealed class DateSelectedListener : Object, IMaterialPickerOnPositiveButtonClickListener
    {
        private readonly DateTimePickerFragment _owner;

        public DateSelectedListener(DateTimePickerFragment owner)
        {
            _owner = owner;
        }

        public void OnPositiveButtonClick(Object? selection)
        {
            var value =
                selection as Long
                ?? throw new InvalidOperationException("The date picker returned no date.");

            _owner.OnDateSelected(value.LongValue());
        }
    }

    private sealed class TimeSelectedListener : Object, View.IOnClickListener
    {
        private readonly DateTimePickerFragment _owner;
        private readonly MaterialTimePicker _picker;

        public TimeSelectedListener(DateTimePickerFragment owner, MaterialTimePicker picker)
        {
            _owner = owner;
            _picker = picker;
        }

        public void OnClick(View? view)
        {
            _owner.OnTimeSelected(_picker.Hour, _picker.Minute);
        }
    }

    private sealed class CloseListener
        : Object,
            View.IOnClickListener,
            Android.Content.IDialogInterfaceOnCancelListener
    {
        private readonly DateTimePickerFragment _owner;

        public CloseListener(DateTimePickerFragment owner)
        {
            _owner = owner;
        }

        public void OnClick(View? view)
        {
            _owner.Close();
        }

        public void OnCancel(Android.Content.IDialogInterface? dialog)
        {
            _owner.Close();
        }
    }
}
