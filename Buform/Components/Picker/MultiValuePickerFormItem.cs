using System.Linq.Expressions;

namespace Buform;

public class MultiValuePickerFormItem<TValue>
    : PickerFormItemBase<IEnumerable<TValue>?>,
        IMultiValuePickerFormItem
{
    private Func<TValue?, string?>? _itemFormatter;
    private IEnumerable<IPickerOptionFormItem> _options;
    private Func<TValue?, string?>? _optionsFilterValueFactory;
    private Func<IEnumerable<TValue>?, string?>? _valueFormatter;
    private IEnumerable<TValue>? _source;

    public override string? FormattedValue => ValueFormatter?.Invoke(Value) ?? Value?.ToString();

    public virtual Func<TValue?, string?>? ItemFormatter
    {
        get => _itemFormatter;
        set
        {
            _itemFormatter = value;

            foreach (var option in Options.OfType<PickerOptionFormItem<TValue>>())
            {
                option.Formatter = ItemFormatter;
            }

            NotifyPropertyChanged();
        }
    }

    public virtual Func<IEnumerable<TValue>?, string?>? ValueFormatter
    {
        get => _valueFormatter;
        set
        {
            _valueFormatter = value;

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FormattedValue));
        }
    }

    public virtual Func<TValue?, string?>? OptionsFilterValueFactory
    {
        get => _optionsFilterValueFactory;
        set
        {
            _optionsFilterValueFactory = value;

            foreach (var option in Options.OfType<PickerOptionFormItem<TValue>>())
            {
                option.FilterValueFactory = _optionsFilterValueFactory;
            }

            NotifyPropertyChanged();
            UpdateOptions();
        }
    }

    public override string? FilterString
    {
        get => base.FilterString;
        set
        {
            base.FilterString = value;

            NotifyPropertyChanged();

            UpdateOptions();
        }
    }

    private void UpdateOptions()
    {
        if (!string.IsNullOrEmpty(FilterString))
        {
            Options = _options.Where(option =>
                option.FilterValue?.Contains(FilterString, StringComparison.OrdinalIgnoreCase)
                ?? false
            );
        }
        else
        {
            Options = _options;
        }

        NotifyPropertyChanged(nameof(Options));
    }

    public virtual IEnumerable<TValue>? Source
    {
        get => _source;
        set
        {
            _source = value;

            _options = _source?.Select(CreateOption) ?? Array.Empty<IPickerOptionFormItem>();

            UpdateOptions();

            NotifyPropertyChanged();
        }
    }

    public MultiValuePickerFormItem(Expression<Func<IEnumerable<TValue>?>> targetProperty)
        : base(targetProperty)
    {
        _options = Array.Empty<IPickerOptionFormItem>();
    }

    protected virtual IPickerOptionFormItem CreateOption(TValue value)
    {
        return new PickerOptionFormItem<TValue>(value) { Formatter = ItemFormatter, FilterValueFactory = OptionsFilterValueFactory};
    }

    public override void Pick(IPickerOptionFormItem? option)
    {
        var value = option?.Value is TValue itemValue ? itemValue : default;

        if (Value == default)
        {
            if (value != null)
            {
                Value = new[] { value };
            }
        }
        else if (value != null)
        {
            Value = Value.Contains(value)
                ? Value.Where(i => !Equals(i, value)).ToArray()
                : Value.Concat(new[] { value! }).ToArray();
        }
    }

    public override bool IsPicked(IPickerOptionFormItem option)
    {
        return Value != default && Value.Contains(option.Value is TValue value ? value : default);
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _itemFormatter = null;
            _valueFormatter = null;
            _source = null;
        }

        base.Dispose(isDisposing);
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
