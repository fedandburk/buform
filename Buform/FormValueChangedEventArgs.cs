namespace Buform;

public sealed class FormValueChangedEventArgs : EventArgs
{
    public string PropertyName { get; }

    public FormValueChangedEventArgs(string propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        PropertyName = propertyName;
    }
}
