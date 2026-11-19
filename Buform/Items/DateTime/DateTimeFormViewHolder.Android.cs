using System.Globalization;
using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class DateTimeFormViewHolder<TItem> : PickerFormViewHolder<TItem>
    where TItem : DateTimeFormItem
{
    private const string DateFormat = "d";
    private const string TimeFormat = "t";
    private const string DateTimeFormat = "g";

    protected DateTimeFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected DateTimeFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override string? GetLabel(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Label;
    }

    protected override string? GetValue(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

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

    protected override Task OnClickAsync()
    {
        if (Item == null)
        {
            throw new InvalidOperationException("The date and time row has no item.");
        }

        var fragmentManager = GetFragmentActivity().SupportFragmentManager;

        DateTimePickerFragment.Show(fragmentManager, Item);

        return Task.CompletedTask;
    }
}

public sealed class DateTimeFormViewHolder : DateTimeFormViewHolder<DateTimeFormItem>
{
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
}
