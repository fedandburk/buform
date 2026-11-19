namespace Buform;

public class FormGroupHandler<TGroup> : IFormGroupHandler
    where TGroup : class, IFormGroup
{
    private TGroup? _group;

    protected TGroup Group =>
        _group ?? throw new InvalidOperationException("The group handler is not initialized.");

    public virtual void Initialize(IFormGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (group is not TGroup typedGroup)
        {
            throw new ArgumentException(
                $"The group must be of type '{typeof(TGroup).FullName}', but it is of type '{group.GetType().FullName}'.",
                nameof(group)
            );
        }

        _group = typedGroup;
    }

    public virtual bool CanSelectItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return false;
    }

    public void SelectItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        OnItemSelected(item);
    }

    protected virtual void OnItemSelected(IFormItem item)
    {
        /* Nothing to do */
    }
}
