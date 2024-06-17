using Fedandburk.Common.Extensions;

namespace Buform.Groups;

public interface IFormGroupHandler
{
    bool CanSelectRow(IFormItem item);
    bool CanEditRow(IFormItem item);
    void OnRowSelected(IFormItem item);
    bool ShouldAutomaticallyDeselectRow(IFormItem item);
    UITableViewCellEditingStyle EditingStyleForRow(IFormItem item);
    void CommitEditingStyle(UITableViewCellEditingStyle editingStyle, IFormItem item);
    bool CanMoveRow(IFormItem item);
    void MoveRow(IFormItem item, int sourceIndex, int destinationIndex);
    bool CanRemoveRow(IFormItem item);
    void RemoveRow(IFormItem item);
    bool CanInsertRow(IFormItem item, int index);
    void InsertRow(IFormItem item, int index);
    void InitializeCell(FormCell cell, IFormItem item);
    void Initialize(IFormGroup group);
}

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

    public virtual bool CanSelectRow(IFormItem item)
    {
        return !item.IsReadOnly;
    }

    public virtual bool CanEditRow(IFormItem item)
    {
        return Group.RemoveCommand.SafeCanExecute(item.Value)
            || Group.InsertCommand.SafeCanExecute(item.Value);
    }

    public virtual void OnRowSelected(IFormItem item)
    {
        /* Nothing to do */
    }

    public bool ShouldAutomaticallyDeselectRow(IFormItem item)
    {
        return !item.IsReadOnly;
    }

    public UITableViewCellEditingStyle EditingStyleForRow(IFormItem item)
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

    public void CommitEditingStyle(UITableViewCellEditingStyle editingStyle, IFormItem item)
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

    public virtual bool CanMoveRow(IFormItem item)
    {
        return Group.MoveCommand.SafeCanExecute(item.Value);
    }

    public virtual void MoveRow(IFormItem item, int sourceIndex, int destinationIndex)
    {
        Group.MoveCommand.SafeCanExecute((sourceIndex, destinationIndex));
    }

    public virtual bool CanRemoveRow(IFormItem item)
    {
        return Group.RemoveCommand.SafeCanExecute(item.Value);
    }

    public virtual void RemoveRow(IFormItem item)
    {
        Group.RemoveCommand.SafeExecute(item.Value);
    }

    public virtual bool CanInsertRow(IFormItem item, int index)
    {
        return Group.InsertCommand.SafeCanExecute(item.Value);
    }

    public virtual void InsertRow(IFormItem item, int index)
    {
        Group.InsertCommand.SafeExecute(item.Value);
    }

    public virtual void InitializeCell(FormCell cell, IFormItem item)
    {
        cell.Initialize(item);
    }
}

public class DefaultFormGroupHandler : FormGroupHandlerBase<IFormGroup>
{
    /* Nothing to do */
}
