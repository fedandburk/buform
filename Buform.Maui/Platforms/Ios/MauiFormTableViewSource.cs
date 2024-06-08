using Foundation;
using UIKit;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class MauiFormTableViewSource : FormTableViewSource
{
    public MauiFormTableViewSource(UITableView tableView)
        : base(tableView)
    {
        /* Required constructor */
    }

    protected override UITableViewCell GetCell(NSIndexPath indexPath, object item)
    {
        var sectionType = item.GetType();

        if (!FormPlatform.TryGetCellViewType(sectionType, out var viewType))
        {
            return base.GetCell(indexPath, item);
        }

        return viewType == null ? base.GetCell(indexPath, item) : new MauiFormCell(viewType, item);
    }

    protected override UITableViewHeaderFooterView? GetViewForFooter(
        nint section,
        object sectionItem
    )
    {
        var sectionType = sectionItem.GetType();

        if (!FormPlatform.TryGetFooterViewType(sectionType, out var viewType))
        {
            return base.GetViewForFooter(section, sectionItem);
        }

        return viewType == null
            ? base.GetViewForFooter(section, sectionItem)
            : new MauiFormHeaderFooter(viewType, sectionItem);
    }

    protected override UITableViewHeaderFooterView? GetViewForHeader(
        nint section,
        object sectionItem
    )
    {
        var sectionType = sectionItem.GetType();

        if (!FormPlatform.TryGetHeaderViewType(sectionType, out var viewType))
        {
            return base.GetViewForHeader(section, sectionItem);
        }

        return viewType == null
            ? base.GetViewForHeader(section, sectionItem)
            : new MauiFormHeaderFooter(viewType, sectionItem);
    }
}
