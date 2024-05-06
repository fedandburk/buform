using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class AsyncPickerFormCell<TItem> : PresentedPickerFormCellBase<TItem>
    where TItem : class, IAsyncPickerFormItem
{
    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected AsyncPickerFormCell()
    {
        /* Required constructor */
    }

    protected AsyncPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override UIViewController CreateViewController(TItem item)
    {
        return new AsyncPickerViewController(UITableViewStyle.InsetGrouped, item);
    }
}

[Preserve(AllMembers = true)]
[Register(nameof(AsyncPickerFormCell))]
public sealed class AsyncPickerFormCell : AsyncPickerFormCell<IAsyncPickerFormItem>
{
    public AsyncPickerFormCell()
    {
        /* Required constructor */
    }

    public AsyncPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
