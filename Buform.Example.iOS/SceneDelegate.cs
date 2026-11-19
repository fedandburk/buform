namespace Buform.Example.iOS;

[Register(nameof(SceneDelegate))]
public sealed class SceneDelegate : UIWindowSceneDelegate
{
    private const string TintColorName = "Main";

    public override UIWindow? Window { get; set; }

    public override void WillConnect(
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
