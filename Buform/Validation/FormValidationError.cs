namespace Buform;

public sealed class FormValidationError
{
    public string TargetPropertyName { get; }

    public string ErrorMessage { get; }

    public FormValidationError(string targetPropertyName, string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetPropertyName);
        ArgumentNullException.ThrowIfNull(errorMessage);

        TargetPropertyName = targetPropertyName;
        ErrorMessage = errorMessage;
    }
}
