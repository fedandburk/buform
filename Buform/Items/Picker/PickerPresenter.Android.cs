using Android.Views;

namespace Buform;

public abstract class PickerPresenter<TItem> : IDisposable
    where TItem : class, IOptionsPickerFormItem
{
    public abstract Task PickAsync(View sourceView, TItem item);

    protected virtual void Dispose(bool disposing)
    {
        /* Nothing to do */
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
