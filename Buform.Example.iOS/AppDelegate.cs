namespace Buform.Example.iOS;

[Register(nameof(AppDelegate))]
public sealed class AppDelegate : UIApplicationDelegate
{
    private const string SceneConfigurationName = "Default Configuration";

    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        ColorPickerFormPlatform.Register();

        FormPlatform.RegisterGroupHeaderNib<LogoFormGroup, LogoFormHeader>();

        FormPlatform.RegisterItem<RandomNumberGeneratorItem, RandomNumberGeneratorCell>();

        return true;
    }

    public override UISceneConfiguration GetConfiguration(
        UIApplication application,
        UISceneSession connectingSceneSession,
        UISceneConnectionOptions options
    )
    {
        return new UISceneConfiguration(SceneConfigurationName, connectingSceneSession.Role)
        {
            DelegateType = typeof(SceneDelegate),
        };
    }

    private static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
