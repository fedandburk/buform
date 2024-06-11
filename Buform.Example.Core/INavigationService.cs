namespace Buform;

public interface INavigationService
{
    Task OpenComponentsAsync();
    Task OpenCreateConnectionAsync();
    Task OpenCreateEventAsync();
}
