namespace Buform.Example.Maui;

public sealed partial class App : Application
{
    private readonly AppShell _shell;

    public App(INavigationService navigationService)
    {
        _shell = (AppShell)navigationService;

        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_shell);
    }
}
