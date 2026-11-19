// ReSharper disable InvertIf

namespace Buform;

public class DialogPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    protected Func<TItem, UIViewController> ViewControllerFactory { get; }

    public DialogPickerPresenter(Func<TItem, UIViewController> viewControllerFactory)
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

        var navigationController = new UINavigationController(pickerViewController);

        navigationController.ModalPresentationStyle = UIModalPresentationStyle.Popover;

        if (navigationController.PopoverPresentationController != null)
        {
            navigationController.PopoverPresentationController.SourceView = sourceView;
            navigationController.PopoverPresentationController.SourceRect = sourceView.Bounds;
        }

        return viewController.PresentViewControllerAsync(navigationController, true);
    }
}
