namespace Buform;

[Register(nameof(AppDelegate))]
public sealed class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    private static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var color = UIColor.FromName("Main");

        UIBarButtonItem.Appearance.TintColor = color;
        UIButton.Appearance.TintColor = color;
        UIDatePicker.Appearance.TintColor = color;
        UISlider.Appearance.TintColor = color;
        UITextField.Appearance.TintColor = color;
        UITextView.Appearance.TintColor = color;

        ColorPickerFormPlatform.Register();

        FormPlatform.RegisterGroupHeaderNib<LogoFormGroup, LogoFormHeader>();

        FormPlatform.RegisterItemClass<RandomNumberGeneratorItem, RandomNumberGeneratorCell>();
        FormPlatform.RegisterItemClass<MvxButtonFormItem, MvxButtonFormCell>();

        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        var viewController = new MenuViewController();
        var navigationController = new UINavigationController(viewController);
        Window.RootViewController = navigationController;

        Window.MakeKeyAndVisible();

        return true;
    }
}
