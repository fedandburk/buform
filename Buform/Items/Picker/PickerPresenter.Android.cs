using Android.Views;

namespace Buform;

public abstract class PickerPresenter<TItem> : IDisposable
    where TItem : class, IOptionsPickerFormItem
{
    public abstract void Pick(View anchorView, TItem item);

    protected virtual void Dispose(bool disposing)
    {
        /* Nothing to dispose */
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
