namespace Buform;

[Preserve(AllMembers = true)]
public class PopUpPickerPresenter<TItem> : PickerPresenterBase<TItem> where TItem : class, IPickerFormItemBase
{
    protected Func<TItem, UIAlertController> AlertControllerFactory { get; }

    public PopUpPickerPresenter(Func<TItem, UIAlertController> alertControllerFactory)
    {
        AlertControllerFactory = alertControllerFactory;
    }

    public override async Task PickAsync(UIView sourceView, TItem item)
    {
        var viewController = GetViewController();

        if (viewController == null)
        {
            return;
        }

        var alertController = AlertControllerFactory(item);

        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
        {
            if (alertController.PopoverPresentationController != null)
            {
                alertController.PopoverPresentationController.SourceView = sourceView;
                alertController.PopoverPresentationController.SourceRect = sourceView.Bounds;
            }
        }

        await viewController.PresentViewControllerAsync(alertController, true).ConfigureAwait(false);
    }
}