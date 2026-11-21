namespace Buform;

public sealed class FormValidationError
{
    public FormValidationError(string propertyName, string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(errorMessage);

        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string PropertyName { get; }

    public string ErrorMessage { get; }
}
