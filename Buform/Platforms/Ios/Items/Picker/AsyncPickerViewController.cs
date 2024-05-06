namespace Buform;

public sealed class AsyncPickerViewController : AsyncPickerViewControllerBase<IAsyncPickerFormItem>
{
    public AsyncPickerViewController(UITableViewStyle style, IAsyncPickerFormItem item)
        : base(style, item)
    {
        /* Required constructor */
    }
}
