using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[Activity(NoHistory = true, MainLauncher = true)]
public class MenuActivity : Activity, INavigationService
{
    private readonly MenuViewModel _viewModel;

    public MenuActivity()
    {
        _viewModel = new MenuViewModel(this);
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.ActivityMenu);

        var recyclerView = FindViewById<FormRecyclerView>(Resource.Id.RecyclerView)!;

        recyclerView.Form = _viewModel.Form;
    }

    public Task OpenComponentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task OpenCreateConnectionAsync()
    {
        throw new NotImplementedException();
    }

    public Task OpenCreateEventAsync()
    {
        throw new NotImplementedException();
    }
}
