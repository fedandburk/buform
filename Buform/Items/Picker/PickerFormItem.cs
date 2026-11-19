using System.Linq.Expressions;

namespace Buform;

public class PickerFormItem<TValue> : OptionsPickerFormItem<TValue>, IPickerFormItem
{
    public virtual Func<TValue?, string?>? Formatter
    {
        get;
        set
        {
            field = value;

            UpdateOptionFormatters();

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FormattedValue));
        }
    }

    public virtual Func<TValue?, string?>? OptionFilterValueFactory
    {
        get;
        set
        {
            field = value;

            UpdateOptionFormatters();

            NotifyPropertyChanged();

            UpdateOptions();
        }
    }

    public virtual IEnumerable<TValue>? Source
    {
        get;
        set
        {
            field = value;

            SetOptions(field?.Select(CreateOption) ?? []);

            NotifyPropertyChanged();
        }
    }

    public override string? FormattedValue => Formatter?.Invoke(Value) ?? Value?.ToString();

    public PickerFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }

    protected virtual IFormOption CreateOption(TValue value)
    {
        return new FormOption<TValue>(value)
        {
            Formatter = Formatter,
            FilterValueFactory = OptionFilterValueFactory,
        };
    }

    private void UpdateOptionFormatters()
    {
        foreach (var option in AllOptions.OfType<FormOption<TValue>>())
        {
            option.Formatter = Formatter;
            option.FilterValueFactory = OptionFilterValueFactory;
        }

        NotifyPropertyChanged(nameof(Options));
    }

    public override void Pick(IFormOption? option)
    {
        Value = option?.Value is TValue value ? value : default;
    }

    public override bool IsPicked(IFormOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        return Equals(Value, option.Value);
    }
}

public class PickerFormItem : PickerFormItem<string?>
{
    public PickerFormItem(Expression<Func<string?>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }
}
