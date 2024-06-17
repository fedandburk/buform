using System.Linq.Expressions;

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

    public TextFormItem(Expression<Func<TValue>> property)
        : base(property)
    {
        /* Required constructor */
    }

    public TextFormItem(TValue value)
        : base(value)
    {
        /* Required constructor */
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        NotifyPropertyChanged(nameof(FormattedValue));
    }
}
