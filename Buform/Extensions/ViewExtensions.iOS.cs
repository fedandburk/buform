namespace Buform;

internal static class ViewExtensions
{
    public static UIViewController? FindViewController(this UIView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        var responder = view.NextResponder;

        while (responder != null)
        {
            if (responder is UIViewController viewController)
            {
                return viewController;
            }

            responder = responder.NextResponder;
        }

        return null;
    }
}
