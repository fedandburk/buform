namespace Buform;

public class TextFormItem<TValue> : FormItem<TValue>, ITextFormItem
{
    private Func<TValue?, string?>? _formatter;

    public virtual Func<TValue?, string?>? Formatter
    {
        get => _formatter;
        set
        {
            _formatter = value;

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FormattedValue));
        }
    }

    public virtual string? FormattedValue => _formatter?.Invoke(Value) ?? Value?.ToString();

    public TextFormItem(TValue value)
        : base(value)
    {
        /* Required constructor */
    }
}
