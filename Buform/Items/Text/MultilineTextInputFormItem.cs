using System.Linq.Expressions;

namespace Buform;

public class MultilineTextInputFormItem<TValue>
    : ValidatableFormItem<TValue>,
        IMultilineTextInputFormItem
{
    private readonly Func<string?, TValue?> _converter;

    private string? _placeholder;
    private TextInputType _inputType;
    private Func<TValue?, string?>? _formatter;

    public virtual string? Placeholder
    {
        get => _placeholder;
        set
        {
            _placeholder = value;

            NotifyPropertyChanged();
        }
    }

    public virtual TextInputType InputType
    {
        get => _inputType;
        set
        {
            _inputType = value;

            NotifyPropertyChanged();
        }
    }

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

    public MultilineTextInputFormItem(
        Expression<Func<TValue>> targetProperty,
        Func<string?, TValue?> converter
    )
        : base(targetProperty)
    {
        ArgumentNullException.ThrowIfNull(converter);

        _converter = converter;
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        NotifyPropertyChanged(nameof(FormattedValue));
    }

    public virtual void SetValue(string? value)
    {
        Value = _converter.Invoke(value);
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _formatter = null;
        }

        base.Dispose(isDisposing);
    }
}

public class MultilineTextInputFormItem : MultilineTextInputFormItem<string?>
{
    public MultilineTextInputFormItem(Expression<Func<string?>> targetProperty)
        : base(targetProperty, item => item)
    {
        /* Required constructor */
    }
}
