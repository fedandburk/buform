namespace Buform.Example.Droid;

[Activity(MainLauncher = true)]
public sealed class MenuActivity : AndroidX.AppCompat.App.AppCompatActivity, INavigationService
{
    private readonly MenuViewModel _viewModel;

    public MenuActivity()
    {
        _viewModel = new MenuViewModel(this);
    }

    public Task OpenComponentsAsync()
    {
        StartActivity(typeof(ComponentsActivity));

        return Task.CompletedTask;
    }

    public Task OpenCreateConnectionAsync()
    {
        StartActivity(typeof(CreateConnectionActivity));

        return Task.CompletedTask;
    }

    public Task OpenCreateEventAsync()
    {
        StartActivity(typeof(CreateEventActivity));

        return Task.CompletedTask;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        AndroidX.Activity.EdgeToEdge.Enable(this);

        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.ActivityMenu);

        var recyclerView = FindViewById<FormRecyclerView>(Resource.Id.RecyclerView)!;

        recyclerView.Form = _viewModel.Form;
    }
}
