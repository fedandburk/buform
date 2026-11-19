namespace Buform.Example.Maui;

public partial class MenuPage : ContentPage
{
    public MenuPage(MenuViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
