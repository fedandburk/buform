// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Buform;

public class DefaultPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    protected Func<TItem, UIViewController> ViewControllerFactory { get; }

    public DefaultPickerPresenter(Func<TItem, UIViewController> viewControllerFactory)
    {
        ViewControllerFactory = viewControllerFactory;
    }

    public override Task PickAsync(UIView sourceView, TItem item)
    {
        var viewController = sourceView.FindViewController();

        if (viewController == null)
        {
            return Task.CompletedTask;
        }

        var pickerViewController = ViewControllerFactory(item);

        if (viewController.NavigationController == null)
        {
            var navigationController = new UINavigationController(pickerViewController);

            return viewController.PresentViewControllerAsync(navigationController, true);
        }

        viewController.NavigationController.PushViewController(pickerViewController, true);

        return Task.CompletedTask;
    }
}
