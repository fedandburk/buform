using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class AsyncPickerFormCellBase<TAsyncPickerItem> : PresentedPickerFormCellBase<TAsyncPickerItem> where TAsyncPickerItem : class, IAsyncPickerFormItem
{
    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected AsyncPickerFormCellBase()
    {
        /* Required constructor */
    }

    protected AsyncPickerFormCellBase(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override UIViewController CreateViewController(TAsyncPickerItem item)
    {
        return new AsyncPickerViewController(UITableViewStyle.InsetGrouped, item);
    }
}
