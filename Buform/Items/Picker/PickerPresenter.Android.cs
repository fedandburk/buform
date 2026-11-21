using Android.Views;

namespace Buform;

public abstract class PickerPresenter<TItem> : IDisposable
    where TItem : class, IOptionsPickerFormItem
{
    public abstract Task PickAsync(View sourceView, TItem item);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        /* Nothing to do */
    }
}
