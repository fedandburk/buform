namespace Buform;

public partial class App : Application
{
    public App(INavigationService navigationService)
    {
        InitializeComponent();

        MainPage = navigationService as AppShell;
    }
}
