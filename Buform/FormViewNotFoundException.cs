namespace Buform;

public sealed class FormViewNotFoundException : Exception
{
    public FormViewNotFoundException(Type type)
        : base($"No view is registered for type '{type?.FullName}'.")
    {
        ArgumentNullException.ThrowIfNull(type);
    }

    public FormViewNotFoundException(int viewType)
        : base($"No view is registered for view type {viewType}.")
    {
        /* Required constructor */
    }
}
