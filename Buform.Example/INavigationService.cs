namespace Buform.Example;

public interface INavigationService
{
    Task OpenComponentsAsync();
    Task OpenCreateConnectionAsync();
    Task OpenCreateEventAsync();
}
