using Fedandburk.iOS.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
public class FormTableViewSource : TableViewSource
{
    private readonly Dictionary<IFormGroup, IFormGroupHandler> _groupHandlers = new();

    public Form? Form
    {
        get => Items as Form;
        set => Items = value;
    }

    public FormTableViewSource(UITableView tableView)
        : base(tableView)
    {
        FormPlatform.Register(TableView);
    }

    private IFormItem? GetItem(NSIndexPath indexPath)
    {
        return Form?.ElementAtOrDefault(indexPath.Section)?.ElementAtOrDefault(indexPath.Row);
    }

    private IFormGroup? GetGroup(nint section)
    {
        return Form?.ElementAtOrDefault((int)section);
    }

    private FormCell? FindCell(NSIndexPath indexPath)
    {
        var formItem = GetItem(indexPath);

        if (formItem == null)
        {
            return GetCell(TableView, indexPath) as FormCell;
        }

        if (TableView.CellAt(indexPath) is not FormCell formCell)
        {
            return GetCell(TableView, indexPath) as FormCell;
        }

        if (!ReferenceEquals(formCell.Item, formItem))
        {
            return GetCell(TableView, indexPath) as FormCell;
        }

        return formCell;
    }

    protected override string? GetHeaderReuseIdentifier(object item)
    {
        return FormPlatform.TryGetHeaderReuseIdentifier(item.GetType(), out var reuseIdentifier)
            ? reuseIdentifier
            : null;
    }

    protected override string? GetFooterReuseIdentifier(object item)
    {
        return FormPlatform.TryGetFooterReuseIdentifier(item.GetType(), out var reuseIdentifier)
            ? reuseIdentifier
            : null;
    }

    protected override string GetCellReuseIdentifier(object item)
    {
        if (!FormPlatform.TryGetReuseIdentifier(item.GetType(), out var reuseIdentifier))
        {
            throw new FormViewNotFoundException(item);
        }

        return reuseIdentifier!;
    }

    protected override string GetExpandedCellReuseIdentifier(object item)
    {
        if (!FormPlatform.TryGetExpandedReuseIdentifier(item.GetType(), out var reuseIdentifier))
        {
            throw new FormViewNotFoundException(item);
        }

        return reuseIdentifier!;
    }

    protected override UITableViewCell GetCell(NSIndexPath indexPath, object item)
    {
        var cell = base.GetCell(indexPath, item);

        if (cell is not FormCell formCell)
        {
            return cell;
        }

        if (item is not IFormItem formItem)
        {
            return cell;
        }

        GetGroupHandler(indexPath).InitializeCell(formCell, formItem);

        return formCell;
    }

    protected override void WillDisplayHeader(
        nint section,
        UITableViewHeaderFooterView view,
        object item
    )
    {
        base.WillDisplayHeader(section, view, item);

        if (view is not FormHeaderFooter headerFooterView)
        {
            return;
        }

        if (item is not IFormGroup formGroup)
        {
            return;
        }

        headerFooterView.Initialize(formGroup);
    }

    protected override void WillDisplayFooter(
        nint section,
        UITableViewHeaderFooterView view,
        object item
    )
    {
        base.WillDisplayFooter(section, view, item);

        if (view is not FormHeaderFooter headerFooterView)
        {
            return;
        }

        if (item is not IFormGroup formGroup)
        {
            return;
        }

        headerFooterView.Initialize(formGroup);
    }

    protected override NSIndexPath WillSelectRow(NSIndexPath indexPath, object item)
    {
        if (item is IFormItem formItem)
        {
            return GetGroupHandler(indexPath).CanSelectItem(formItem) ? indexPath : null!;
        }

        return null!;
    }

    protected override bool ShouldBeAutomaticallyDeselected(NSIndexPath indexPath, object item)
    {
        if (item is not IFormItem formItem)
        {
            return false;
        }

        return GetGroupHandler(indexPath).ShouldAutomaticallyDeselectItem(formItem);
    }

    protected override void RowSelected(NSIndexPath indexPath, object item)
    {
        if (item is IFormItem formItem)
        {
            GetGroupHandler(indexPath).OnItemSelected(formItem);
        }

        FindCell(indexPath)?.OnSelected();
    }

    protected override bool CanEditRow(NSIndexPath indexPath, object item)
    {
        if (item is not IFormItem formItem)
        {
            return false;
        }

        return GetGroupHandler(indexPath).CanEditItem(formItem);
    }

    protected override UITableViewCellEditingStyle EditingStyleForRow(
        NSIndexPath indexPath,
        object item
    )
    {
        if (item is not IFormItem formItem)
        {
            return UITableViewCellEditingStyle.None;
        }

        return GetGroupHandler(indexPath).EditingStyleForItem(formItem);
    }

    protected override void CommitEditingStyle(
        UITableViewCellEditingStyle editingStyle,
        NSIndexPath indexPath,
        object item
    )
    {
        if (item is not IFormItem formItem)
        {
            return;
        }

        GetGroupHandler(indexPath).CommitEditingStyleForItem(editingStyle, formItem);
    }

    protected override bool CanMoveRow(NSIndexPath indexPath, object item)
    {
        if (item is not IFormItem formItem)
        {
            return false;
        }

        return GetGroupHandler(indexPath).CanMoveItem(formItem);
    }

    protected override void MoveRow(
        NSIndexPath sourceIndexPath,
        NSIndexPath destinationIndexPath,
        object item
    )
    {
        var sourceGroupHandler = GetGroupHandler(sourceIndexPath);
        var destinationGroupHandler = GetGroupHandler(destinationIndexPath);

        var formItem = GetItem(sourceIndexPath)!;

        if (sourceGroupHandler == destinationGroupHandler)
        {
            sourceGroupHandler.MoveItem(formItem, sourceIndexPath.Row, destinationIndexPath.Row);
        }
        else
        {
            sourceGroupHandler.RemoveItem(formItem);
            destinationGroupHandler.InsertItem(formItem, destinationIndexPath.Row);
        }
    }

    public override NSIndexPath CustomizeMoveTarget(
        UITableView tableView,
        NSIndexPath sourceIndexPath,
        NSIndexPath proposedIndexPath
    )
    {
        var sourceGroupHandler = GetGroupHandler(sourceIndexPath);
        var proposedGroupHandler = GetGroupHandler(proposedIndexPath);

        var formItem = GetItem(sourceIndexPath)!;

        if (sourceGroupHandler == proposedGroupHandler)
        {
            return proposedIndexPath;
        }

        var targetGroup = GetGroup(proposedIndexPath.Section)!;

        if (
            sourceGroupHandler.CanMoveItemIntoGroup(formItem, targetGroup)
            && proposedGroupHandler.CanInsertItem(formItem)
        )
        {
            return proposedIndexPath;
        }

        return sourceIndexPath;
    }

    private IFormGroupHandler GetGroupHandler(NSIndexPath indexPath)
    {
        var group = GetGroup(indexPath.Section)!;

        if (_groupHandlers.TryGetValue(group, out var groupHandler))
        {
            return groupHandler;
        }

        groupHandler = FormPlatform.GetGroupHandler(group);

        groupHandler.Initialize(group);

        _groupHandlers[group] = groupHandler;

        return groupHandler;
    }
}
