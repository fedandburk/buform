namespace Buform;

public static class UiApplicationExtensions
{
    public static UIViewController? GetTopViewController(this UIApplication application)
    {
        var window = application.ConnectedScenes
            .ToArray()
            .OfType<UIWindowScene>()
            .Select(item => item.KeyWindow)
            .FirstOrDefault();
        
        var topViewController = window?.RootViewController;

        if (topViewController == null)
        {
            return null;
        }

        while (topViewController.PresentedViewController != null)
        {
            topViewController = topViewController.PresentedViewController;
        }

        var navigationController = topViewController as UINavigationController;

        if (navigationController != null)
        {
            topViewController = navigationController.ViewControllers?.LastOrDefault();
        }

        return topViewController ?? navigationController;
    }
}