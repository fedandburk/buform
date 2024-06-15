using Foundation;
using UIKit;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class MauiFormTableViewSource : FormTableViewSource
{
    public MauiFormTableViewSource(UITableView tableView)
        : base(tableView)
    {
        TableView.RegisterClassForCellReuse(typeof(MauiFormCell), nameof(MauiFormCell));

        TableView.RegisterClassForHeaderFooterViewReuse(
            typeof(MauiFormHeaderFooterView),
            nameof(MauiFormHeaderFooterView)
        );
    }

    protected override UITableViewCell GetCell(NSIndexPath indexPath, object item)
    {
        var sectionType = item.GetType();

        if (!MauiFormPlatform.TryGetCellViewType(sectionType, out var viewType))
        {
            return base.GetCell(indexPath, item);
        }

        if (viewType == null)
        {
            return base.GetCell(indexPath, item);
        }

        var cell = TableView.DequeueReusableCell(nameof(MauiFormCell), indexPath);

        if (cell is MauiFormCell mauiCell)
        {
            mauiCell.Initialize(viewType, item);
        }

        return cell;
    }

    protected override UITableViewHeaderFooterView? GetViewForFooter(
        nint section,
        object sectionItem
    )
    {
        var sectionType = sectionItem.GetType();

        if (!MauiFormPlatform.TryGetFooterViewType(sectionType, out var viewType))
        {
            return base.GetViewForFooter(section, sectionItem);
        }

        if (viewType == null)
        {
            return base.GetViewForFooter(section, sectionItem);
        }

        var view = TableView.DequeueReusableHeaderFooterView(nameof(MauiFormHeaderFooterView));

        if (view is MauiFormHeaderFooterView mauiHeaderFooterView)
        {
            mauiHeaderFooterView.Initialize(viewType, sectionItem);
        }

        return view;
    }

    protected override UITableViewHeaderFooterView? GetViewForHeader(
        nint section,
        object sectionItem
    )
    {
        var sectionType = sectionItem.GetType();

        if (!MauiFormPlatform.TryGetHeaderViewType(sectionType, out var viewType))
        {
            return base.GetViewForHeader(section, sectionItem);
        }

        if (viewType == null)
        {
            return base.GetViewForHeader(section, sectionItem);
        }

        var view = TableView.DequeueReusableHeaderFooterView(nameof(MauiFormHeaderFooterView));

        if (view is MauiFormHeaderFooterView mauiHeaderFooterView)
        {
            mauiHeaderFooterView.Initialize(viewType, sectionItem);
        }

        return view;
    }
}
