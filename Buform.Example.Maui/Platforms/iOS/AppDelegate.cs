using Foundation;

namespace Buform.Example.Maui;

[Register(nameof(AppDelegate))]
public sealed class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        ColorPickerFormPlatform.Register();

        return MauiProgram.CreateMauiApp();
    }
}
