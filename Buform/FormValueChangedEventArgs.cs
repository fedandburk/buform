namespace Buform;

public sealed class FormValueChangedEventArgs : EventArgs
{
    public FormValueChangedEventArgs(string propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}
