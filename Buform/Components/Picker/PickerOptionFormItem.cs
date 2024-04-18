namespace Buform;

public class PickerOptionFormItem<TValue> : FormItem<TValue>, IPickerOptionFormItem
{
    private Func<TValue?, string?>? _formatter;
    private Func<TValue?, string?>? _filterValueFactory;

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

    public virtual Func<TValue?, string?>? FilterValueFactory
    {
        get => _filterValueFactory;
        set
        {
            _filterValueFactory = value;

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FilterValue));
        }
    }

    public virtual string? FormattedValue => _formatter?.Invoke(Value) ?? Value?.ToString();
    public virtual string? FilterValue => _filterValueFactory?.Invoke(Value) ?? Value?.ToString();

    public PickerOptionFormItem(TValue value)
        : base(value)
    {
        /* Required constructor */
    }
}
