namespace Buform;

public class FormGroupHandler<TGroup> : IFormGroupHandler
    where TGroup : class, IFormGroup
{
    private TGroup? _group;
    private UITableView? _tableView;

    protected TGroup Group =>
        _group ?? throw new InvalidOperationException("The group handler is not initialized.");

    protected UITableView TableView =>
        _tableView ?? throw new InvalidOperationException("The group handler is not initialized.");

    public virtual void Initialize(IFormGroup group, UITableView tableView)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(tableView);

        if (group is not TGroup typedGroup)
        {
            throw new ArgumentException(
                $"The group must be of type '{typeof(TGroup).FullName}'.",
                nameof(group)
            );
        }

        _group = typedGroup;
        _tableView = tableView;
    }

    public virtual bool CanSelectItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return !item.IsReadOnly;
    }

    public virtual bool CanEditItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return CanRemoveItem(item) || CanInsertItem(item);
    }

    public virtual void OnItemSelected(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        /* Nothing to do */
    }

    public virtual void OnItemDeselected(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        /* Nothing to do */
    }

    public virtual bool ShouldAutomaticallyDeselectItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return !item.IsReadOnly;
    }

    public virtual UITableViewCellEditingStyle EditingStyleForItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (CanRemoveItem(item))
        {
            return UITableViewCellEditingStyle.Delete;
        }

        if (CanInsertItem(item))
        {
            return UITableViewCellEditingStyle.Insert;
        }

        return UITableViewCellEditingStyle.None;
    }

    public virtual void CommitEditingStyleForItem(
        UITableViewCellEditingStyle editingStyle,
        IFormItem item
    )
    {
        ArgumentNullException.ThrowIfNull(item);

        switch (editingStyle)
        {
            case UITableViewCellEditingStyle.None:
                return;
            case UITableViewCellEditingStyle.Delete:
                Group.RemoveCommand.SafeExecute(item.Value);
                break;
            case UITableViewCellEditingStyle.Insert:
                Group.InsertCommand.SafeExecute(item.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(editingStyle), editingStyle, null);
        }
    }

    public virtual bool CanMoveItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Group.MoveCommand is not null;
    }

    public virtual bool CanMoveItemIntoGroup(IFormItem item, IFormGroup targetGroup)
    {
        ArgumentNullException.ThrowIfNull(item);

        return false;
    }

    public virtual void MoveItem(IFormItem item, int sourceIndex, int destinationIndex)
    {
        ArgumentNullException.ThrowIfNull(item);

        Group.MoveCommand.SafeExecute(FormItemMove.Create(Group, sourceIndex, destinationIndex));
    }

    public virtual bool CanRemoveItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Group.RemoveCommand.SafeCanExecute(item.Value);
    }

    public virtual void RemoveItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Group.RemoveCommand?.SafeExecute(item.Value);
    }

    public virtual bool CanInsertItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Group.InsertCommand.SafeCanExecute(item.Value);
    }

    public virtual void InsertItem(IFormItem item, int index)
    {
        ArgumentNullException.ThrowIfNull(item);

        Group.InsertCommand?.SafeExecute(item.Value);
    }

    public virtual void InitializeCell(FormCell cell, NSIndexPath indexPath, IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(cell);
        ArgumentNullException.ThrowIfNull(indexPath);
        ArgumentNullException.ThrowIfNull(item);

        cell.Initialize(item);
    }
}
