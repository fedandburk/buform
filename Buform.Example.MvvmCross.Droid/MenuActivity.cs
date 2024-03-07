using Android.Runtime;
using Buform.Example.Core;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Buform.Example.MvvmCross.Droid;

[Preserve(AllMembers = true)]
[MvxActivityPresentation]
[Activity]
public class MenuActivity : MvxActivity<MenuViewModel>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.ActivityMenu);
    }
}