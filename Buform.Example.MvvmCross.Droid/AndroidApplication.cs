using Android;
using Android.Runtime;
using MvvmCross.Platforms.Android.Views;

[assembly: UsesPermission(Manifest.Permission.Internet)]

namespace Buform.Example.MvvmCross.Droid;

[Application(Theme = "@style/Theme.Material3.DayNight")]
// ReSharper disable once UnusedType.Global
public sealed class AndroidApplication : MvxAndroidApplication<Setup, Core.Application>
{
    public AndroidApplication(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }
}
