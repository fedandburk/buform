namespace Buform;

public interface IMultilineTextInputFormItem : IValidatableFormItem
{
    string? Placeholder { get; }

    TextInputType InputType { get; }

    string? FormattedValue { get; }

    void SetValue(string? value);
}
