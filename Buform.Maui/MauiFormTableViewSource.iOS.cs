using Foundation;
using UIKit;

namespace Buform;

internal sealed class MauiFormTableViewSource : FormTableViewSource
{
    public MauiFormTableViewSource(UITableView tableView)
        : base(tableView)
    {
        tableView.RegisterClassForCellReuse(typeof(MauiFormCell), nameof(MauiFormCell));
        tableView.RegisterClassForHeaderFooterViewReuse(
            typeof(MauiFormHeaderFooterView),
            nameof(MauiFormHeaderFooterView)
        );
    }

    protected override UITableViewCell GetCell(
        UITableView tableView,
        NSIndexPath indexPath,
        IFormItem item
    )
    {
        if (!MauiFormPlatform.TryGetCellViewType(item.GetType(), out var viewType))
        {
            return base.GetCell(tableView, indexPath, item);
        }

        var cell = (MauiFormCell)tableView.DequeueReusableCell(nameof(MauiFormCell), indexPath);

        cell.Initialize(viewType!, item);

        return cell;
    }

    protected override UITableViewHeaderFooterView? GetHeaderView(
        UITableView tableView,
        IFormGroup group
    )
    {
        if (!MauiFormPlatform.TryGetHeaderViewType(group.GetType(), out var viewType))
        {
            return base.GetHeaderView(tableView, group);
        }

        var view = (MauiFormHeaderFooterView)
            tableView.DequeueReusableHeaderFooterView(nameof(MauiFormHeaderFooterView))!;

        view.Initialize(viewType!, group);

        return view;
    }

    protected override UITableViewHeaderFooterView? GetFooterView(
        UITableView tableView,
        IFormGroup group
    )
    {
        if (!MauiFormPlatform.TryGetFooterViewType(group.GetType(), out var viewType))
        {
            return base.GetFooterView(tableView, group);
        }

        var view = (MauiFormHeaderFooterView)
            tableView.DequeueReusableHeaderFooterView(nameof(MauiFormHeaderFooterView))!;

        view.Initialize(viewType!, group);

        return view;
    }
}
