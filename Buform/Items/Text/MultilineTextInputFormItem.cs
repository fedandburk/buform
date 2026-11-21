using System.Linq.Expressions;

namespace Buform;

public class MultilineTextInputFormItem<TValue>
    : ValidatableFormItem<TValue>,
        IMultilineTextInputFormItem
{
    private readonly Func<string?, TValue?> _converter;

    private Func<TValue?, string?>? _formatter;

    public MultilineTextInputFormItem(
        Expression<Func<TValue>> targetProperty,
        Func<string?, TValue?> converter
    )
        : base(targetProperty)
    {
        ArgumentNullException.ThrowIfNull(converter);

        _converter = converter;
    }

    public virtual string? Placeholder
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual TextInputType InputType
    {
        get;
        set
        {
            field = value;

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

    public virtual void SetText(string? text)
    {
        Value = _converter.Invoke(text);
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        NotifyPropertyChanged(nameof(FormattedValue));
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
