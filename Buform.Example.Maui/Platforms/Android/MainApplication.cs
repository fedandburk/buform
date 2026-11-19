using Android.App;
using Android.Runtime;

namespace Buform.Example.Maui;

[Application]
public sealed class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership) { }

    protected override MauiApp CreateMauiApp()
    {
        ColorPickerFormPlatform.Register();

        return MauiProgram.CreateMauiApp();
    }
}
