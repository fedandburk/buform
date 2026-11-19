using Android.Views;
using AndroidX.Activity;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using Google.Android.Material.AppBar;

namespace Buform.Example.Droid;

public abstract class FormActivity : AppCompatActivity
{
    protected MaterialToolbar? Toolbar { get; private set; }

    protected abstract Form Form { get; }

    protected virtual void OnCreateMenu(IMenu menu)
    {
        /* Nothing to do */
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        EdgeToEdge.Enable(this);

        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.ActivityForm);

        Toolbar = FindViewById<MaterialToolbar>(Resource.Id.Toolbar)!;
        Toolbar.Title = Title;
        Toolbar.SetNavigationIcon(Resource.Drawable.abc_ic_ab_back_material);
        Toolbar.NavigationClick += (_, _) => Finish();

        OnCreateMenu(Toolbar.Menu!);

        var recyclerView = FindViewById<FormRecyclerView>(Resource.Id.RecyclerView)!;

        ViewCompat.SetOnApplyWindowInsetsListener(recyclerView, new BottomInsetListener());

        recyclerView.Form = Form;
    }

    private sealed class BottomInsetListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(View? view, WindowInsetsCompat? insets)
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(insets);

            var types = WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.Ime();
            var bottom = insets.GetInsets(types)!.Bottom;

            view.SetPadding(view.PaddingLeft, view.PaddingTop, view.PaddingRight, bottom);

            return insets;
        }
    }
}
