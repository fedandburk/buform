namespace Buform;

public interface ICallbackPickerFormItem : IValidatableFormItem
{
    string? Label { get; }

    string? FormattedValue { get; }

    Task ExecuteCallbackAsync();
}
