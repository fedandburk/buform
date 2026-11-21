namespace Buform;

public class DialogPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    public DialogPickerPresenter(Func<TItem, UIViewController> viewControllerFactory)
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

        var navigationController = new UINavigationController(pickerViewController)
        {
            ModalPresentationStyle = UIModalPresentationStyle.Popover,
        };

        var popoverPresentationController = navigationController.PopoverPresentationController;

        if (popoverPresentationController != null)
        {
            popoverPresentationController.SourceView = sourceView;
            popoverPresentationController.SourceRect = sourceView.Bounds;
        }

        return viewController.PresentViewControllerAsync(navigationController, true);
    }
}
