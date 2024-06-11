namespace Buform;

public partial class ComponentsPage : ContentPage
{
    public ComponentsPage(ComponentsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
