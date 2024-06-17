using Fedandburk.Common.Extensions;

namespace Buform.Groups;

public abstract class FormGroupHandlerBase<TGroup> : IFormGroupHandler
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
        return Group.RemoveCommand.SafeCanExecute(item.Value)
            || Group.InsertCommand.SafeCanExecute(item.Value);
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
        if (Group.RemoveCommand.SafeCanExecute(item.Value))
        {
            return UITableViewCellEditingStyle.Delete;
        }

        if (Group.InsertCommand.SafeCanExecute(item.Value))
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
        return Group.MoveCommand.SafeCanExecute(item.Value);
    }

    public virtual void MoveItem(IFormItem item, int sourceIndex, int destinationIndex)
    {
        Group.MoveCommand?.Execute((sourceIndex, destinationIndex));
    }

    public virtual bool CanRemoveItem(IFormItem item)
    {
        return Group.RemoveCommand.SafeCanExecute(item.Value);
    }

    public virtual void RemoveItem(IFormItem item)
    {
        Group.RemoveCommand?.SafeExecute(item.Value);
    }

    public virtual bool CanInsertItem(IFormItem item, int index)
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
