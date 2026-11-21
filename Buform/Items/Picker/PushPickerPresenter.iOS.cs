namespace Buform;

public class PushPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    public PushPickerPresenter(Func<TItem, UIViewController> viewControllerFactory)
    {
        ArgumentNullException.ThrowIfNull(viewControllerFactory);

        ViewControllerFactory = viewControllerFactory;
    }

    protected Func<TItem, UIViewController> ViewControllerFactory { get; }

    public override Task PickAsync(UIView sourceView, TItem item)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        ArgumentNullException.ThrowIfNull(item);

        var viewController = sourceView.FindViewController();

        if (viewController == null)
        {
            throw new InvalidOperationException("The picker row has no view controller.");
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
