namespace Buform;

public interface IMultilineTextInputFormItem : IValidatableFormItem
{
    string? Placeholder { get; }

    TextInputType InputType { get; }

    string? FormattedValue { get; }

    void SetText(string? text);
}
