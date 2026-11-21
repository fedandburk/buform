namespace Buform.Example.Maui;

public partial class ComponentsPage : ContentPage
{
    public ComponentsPage(ComponentsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
