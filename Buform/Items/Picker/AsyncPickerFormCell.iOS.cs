using ObjCRuntime;

namespace Buform;

public abstract class AsyncPickerFormCell<TItem> : PresentedPickerFormCell<TItem>
    where TItem : class, IAsyncPickerFormItem
{
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
        ArgumentNullException.ThrowIfNull(item);

        return new AsyncPickerViewController(UITableViewStyle.InsetGrouped, item);
    }
}

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
