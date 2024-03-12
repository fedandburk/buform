using Fedandburk.iOS.Extensions;

namespace Buform;

public abstract class PickerPresenterBase<TItem> : NSObject
    where TItem : class, IPickerFormItemBase
{
    protected static UIViewController? GetViewController()
    {
        return UIApplication.SharedApplication.GetTopViewController();
    }

    public abstract Task PickAsync(UIView sourceView, TItem item);
}
