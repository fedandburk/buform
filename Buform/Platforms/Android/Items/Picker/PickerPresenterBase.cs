using Android.Views;

namespace Buform;

public abstract class PickerPresenterBase<TItem> : Java.Lang.Object
    where TItem : class
{
    public abstract void Pick(View anchorView, TItem item);
}
