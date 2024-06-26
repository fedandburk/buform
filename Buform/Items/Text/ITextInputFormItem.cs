namespace Buform;

public interface ITextInputFormItem : IValidatableFormItem
{
    string? Label { get; }

    string? Placeholder { get; }

    TextInputType InputType { get; }

    bool IsSecured { get; }

    string? FormattedValue { get; }

    void SetValue(string? value);
}
