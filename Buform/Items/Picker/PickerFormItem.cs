using System.Linq.Expressions;

namespace Buform;

public class PickerFormItem<TValue> : PickerFormItemBase<TValue>, IPickerFormItem
{
    private Func<TValue?, string?>? _formatter;
    private Func<TValue?, string?>? _optionsFilterValueFactory;
    private IEnumerable<TValue>? _source;
    private IList<IPickerOptionFormItem> _options;

    public virtual Func<TValue?, string?>? Formatter
    {
        get => _formatter;
        set
        {
            _formatter = value;

            foreach (var option in Options.OfType<PickerOptionFormItem<TValue>>())
            {
                option.Formatter = Formatter;
            }

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

    public virtual IEnumerable<TValue>? Source
    {
        get => _source;
        set
        {
            _source = value;

            _options =
                _source?.Select(CreateOption).ToList()
                ?? Array.Empty<IPickerOptionFormItem>().ToList();

            UpdateOptions();

            NotifyPropertyChanged();
        }
    }

    private void UpdateOptions()
    {
        if (!string.IsNullOrEmpty(FilterQuery))
        {
            Options = _options
                .Where(option =>
                    option.FilterValue?.Contains(FilterQuery, StringComparison.OrdinalIgnoreCase)
                    ?? false
                )
                .ToList();
        }
        else
        {
            Options = _options;
        }

        NotifyPropertyChanged(nameof(Options));
    }

    public override string? FilterQuery
    {
        get => base.FilterQuery;
        set
        {
            base.FilterQuery = value;

            NotifyPropertyChanged();

            UpdateOptions();
        }
    }

    public override string? FormattedValue => Formatter?.Invoke(Value) ?? Value?.ToString();

    public PickerFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        _options = Array.Empty<IPickerOptionFormItem>();
    }

    protected virtual IPickerOptionFormItem CreateOption(TValue value)
    {
        return new PickerOptionFormItem<TValue>(value)
        {
            Formatter = Formatter,
            FilterValueFactory = OptionsFilterValueFactory
        };
    }

    public override void Pick(IPickerOptionFormItem? item)
    {
        Value = (TValue?)item?.Value;
    }

    public override bool IsPicked(IPickerOptionFormItem item)
    {
        return Equals(Value, item.Value);
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _formatter = null;
            _source = null;
        }

        base.Dispose(isDisposing);
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
