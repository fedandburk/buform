namespace Buform;

public partial class AppShell : Shell, INavigationService
{
    public AppShell()
    {
        InitializeComponent();
    }

    private Task GoToAsync<T>(params (string Key, object? Value)[] query)
        where T : Page
    {
        var route = typeof(T).Name;
        var parameters = query.ToDictionary(item => item.Key, item => item.Value);

        return MainThread.InvokeOnMainThreadAsync(() => GoToAsync(route, true, parameters));
    }

    public Task OpenComponentsAsync()
    {
        return GoToAsync<ComponentsPage>();
    }

    public Task OpenCreateConnectionAsync()
    {
        return GoToAsync<CreateConnectionPage>();
    }

    public Task OpenCreateEventAsync()
    {
        return GoToAsync<CreateEventPage>();
    }
}
