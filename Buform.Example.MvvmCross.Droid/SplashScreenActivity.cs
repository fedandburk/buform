using Android.Runtime;
using MvvmCross.Platforms.Android.Views;

namespace Buform.Example.MvvmCross.Droid;

[Preserve(AllMembers = true)]
[Activity(
    NoHistory = true,
    MainLauncher = true
)]
public sealed class SplashScreenActivity : MvxStartActivity
{
}