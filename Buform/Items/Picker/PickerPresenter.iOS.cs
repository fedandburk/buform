namespace Buform;

public abstract class PickerPresenter<TItem> : NSObject
    where TItem : class, IOptionsPickerFormItem
{
    public abstract Task PickAsync(UIView sourceView, TItem item);
}
