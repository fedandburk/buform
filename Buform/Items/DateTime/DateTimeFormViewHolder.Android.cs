using System.Globalization;
using Android.Runtime;
using Android.Views;

namespace Buform;

public class DateTimeFormViewHolder : PickerFormViewHolder<DateTimeFormItem>
{
    private const string DateFormat = "d";
    private const string TimeFormat = "t";
    private const string DateTimeFormat = "g";

    public DateTimeFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public DateTimeFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override string? GetLabel(DateTimeFormItem item)
    {
        return item.Label;
    }

    protected override string? GetValue(DateTimeFormItem item)
    {
        if (item.Value is not { } value)
        {
            return null;
        }

        var format = item.InputType switch
        {
            DateTimeInputType.Date => DateFormat,
            DateTimeInputType.Time => TimeFormat,
            DateTimeInputType.DateTime => DateTimeFormat,
            _ => throw new ArgumentOutOfRangeException(
                nameof(DateTimeFormItem.InputType),
                item.InputType,
                null
            ),
        };

        return value.ToString(format, CultureInfo.CurrentCulture);
    }

    protected override void OnClick()
    {
        if (Data == null)
        {
            return;
        }

        DateTimePickerFragment.Show(ItemView, Data);
    }
}
