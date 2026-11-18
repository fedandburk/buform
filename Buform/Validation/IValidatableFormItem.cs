namespace Buform;

public interface IValidatableFormItem : IFormItem
{
    string? ValidationErrorMessage { get; }

    void SetValidationError(FormValidationError? validationError, bool showsUnchangedValue);
    void ResetValidationError();
}
