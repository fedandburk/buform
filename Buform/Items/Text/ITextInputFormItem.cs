namespace Buform;

public interface ITextInputFormItem : IValidatableFormItem
{
    string? Label { get; }

    string? Placeholder { get; }

    TextInputType InputType { get; }

    bool IsSecure { get; }

    string? FormattedValue { get; }

    void SetText(string? text);
}
