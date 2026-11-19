namespace Buform.Example.iOS;

[Register(nameof(SceneDelegate))]
public sealed class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    private const string TintColorName = "Main";

    [Export("window")]
    public UIWindow? Window { get; set; }

    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(
        UIScene scene,
        UISceneSession session,
        UISceneConnectionOptions connectionOptions
    )
    {
        if (scene is not UIWindowScene windowScene)
        {
            return;
        }

        var viewController = new MenuViewController();

        Window = new UIWindow(windowScene)
        {
            RootViewController = new UINavigationController(viewController),
            TintColor = UIColor.FromName(TintColorName),
        };

        Window.MakeKeyAndVisible();
    }
}
