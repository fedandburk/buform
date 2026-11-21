namespace Buform;

public abstract class PickerPresenter<TItem> : IDisposable
    where TItem : class, IOptionsPickerFormItem
{
    public abstract Task PickAsync(UIView sourceView, TItem item);

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        /* Nothing to do */
    }
}
