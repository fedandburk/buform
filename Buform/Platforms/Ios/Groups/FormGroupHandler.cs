using Fedandburk.Common.Extensions;

namespace Buform;

public class FormGroupHandler<TGroup> : IFormGroupHandler
    where TGroup : IFormGroup
{
    protected TGroup Group { get; set; } = default!;

    public virtual void Initialize(IFormGroup group)
    {
        if (group is TGroup tGroup)
        {
            Group = tGroup;
        }
    }

    public virtual bool CanSelectItem(IFormItem item)
    {
        return !item.IsReadOnly;
    }

    public virtual bool CanEditItem(IFormItem item)
    {
        return CanRemoveItem(item) || CanInsertItem(item);
    }

    public virtual void OnItemSelected(IFormItem item)
    {
        /* Nothing to do */
    }

    public bool ShouldAutomaticallyDeselectItem(IFormItem item)
    {
        return !item.IsReadOnly;
    }

    public UITableViewCellEditingStyle EditingStyleForItem(IFormItem item)
    {
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

    public void CommitEditingStyleForItem(UITableViewCellEditingStyle editingStyle, IFormItem item)
    {
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
        return Group.MoveCommand is not null;
    }

    public virtual bool CanMoveItemIntoGroup(IFormItem item, IFormGroup targetGroup)
    {
        return false;
    }

    public virtual void MoveItem(IFormItem item, int sourceIndex, int destinationIndex)
    {
        Group.MoveCommand?.SafeExecute((sourceIndex, destinationIndex));
    }

    public virtual bool CanRemoveItem(IFormItem item)
    {
        return Group.RemoveCommand.SafeCanExecute(item.Value);
    }

    public virtual void RemoveItem(IFormItem item)
    {
        Group.RemoveCommand?.SafeExecute(item.Value);
    }

    public virtual bool CanInsertItem(IFormItem item)
    {
        return Group.InsertCommand.SafeCanExecute(item.Value);
    }

    public virtual void InsertItem(IFormItem item, int index)
    {
        Group.InsertCommand?.SafeExecute(item.Value);
    }

    public virtual void InitializeCell(FormCell cell, IFormItem item)
    {
        cell.Initialize(item);
    }
}
