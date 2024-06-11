using Android;
using Android.Runtime;

[assembly: UsesPermission(Manifest.Permission.Internet)]

namespace Buform;

[Application(Theme = "@style/Theme.Material3.DayNight")]
// ReSharper disable once UnusedType.Global
public sealed class AndroidApplication : Application
{
    public AndroidApplication(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public override void OnCreate()
    {
        base.OnCreate();

        FormPlatform.RegisterGroupHeader<LogoFormGroup, LogoFormGroupHeaderViewHolder>(
            Resource.Layout.FormGroupTextHeaderLayout
        );

        FormPlatform.RegisterGroupFooter<LogoFormGroup, LogoFormGroupFooterViewHolder>(
            Resource.Layout.FormGroupTextFooterLayout
        );
    }
}
