using System.Linq.Expressions;

namespace Buform;

public class MultiValuePickerFormItem<TValue>
    : OptionsPickerFormItem<IEnumerable<TValue>?>,
        IMultiValuePickerFormItem
{
    public override string? FormattedValue => ValueFormatter?.Invoke(Value) ?? Value?.ToString();

    public virtual Func<TValue, string?>? OptionFormatter
    {
        get;
        set
        {
            field = value;

            UpdateOptionFormatters();

            NotifyPropertyChanged();
        }
    }

    public virtual Func<IEnumerable<TValue>?, string?>? ValueFormatter
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FormattedValue));
        }
    }

    public virtual Func<TValue, string?>? OptionFilterValueFactory
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

    public MultiValuePickerFormItem(Expression<Func<IEnumerable<TValue>?>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }

    protected virtual IFormOption CreateOption(TValue value)
    {
        return new FormOption<TValue>(value)
        {
            Formatter = OptionFormatter,
            FilterValueFactory = OptionFilterValueFactory,
        };
    }

    private void UpdateOptionFormatters()
    {
        foreach (var option in AllOptions.OfType<FormOption<TValue>>())
        {
            option.Formatter = OptionFormatter;
            option.FilterValueFactory = OptionFilterValueFactory;
        }

        NotifyPropertyChanged(nameof(Options));
    }

    public override void Pick(IFormOption? option)
    {
        if (option?.Value is not TValue value)
        {
            return;
        }

        var values = Value?.ToList() ?? [];

        if (!values.Remove(value))
        {
            values.Add(value);
        }

        Value = values;
    }

    public override bool IsPicked(IFormOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        return option.Value is TValue value && (Value?.Contains(value) ?? false);
    }
}

public class MultiValuePickerFormItem : MultiValuePickerFormItem<string>
{
    public MultiValuePickerFormItem(Expression<Func<IEnumerable<string>?>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }
}
