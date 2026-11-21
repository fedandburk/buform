namespace Buform;

internal sealed class FormGroupHandlerList
{
    private readonly Dictionary<IFormGroup, IFormGroupHandler> _groupHandlers;

    public FormGroupHandlerList()
    {
        _groupHandlers = [];
    }

    public IFormGroupHandler Get(IFormGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (_groupHandlers.TryGetValue(group, out var groupHandler))
        {
            return groupHandler;
        }

        groupHandler = FormPlatform.GetGroupHandler(group);
        groupHandler.Initialize(group);

        _groupHandlers[group] = groupHandler;

        return groupHandler;
    }

    public void Clear()
    {
        _groupHandlers.Clear();
    }
}
