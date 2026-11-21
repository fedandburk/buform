namespace Buform;

public class FormOption<TValue> : IFormOption
{
    public FormOption(TValue value)
    {
        Value = value;
    }

    object? IFormOption.Value => Value;

    public TValue Value { get; }

    public virtual Func<TValue, string?>? Formatter { get; set; }

    public virtual Func<TValue, string?>? FilterValueFactory { get; set; }

    public virtual string? FormattedValue => Formatter?.Invoke(Value) ?? Value?.ToString();

    public virtual string? FilterValue => FilterValueFactory?.Invoke(Value) ?? FormattedValue;
}
