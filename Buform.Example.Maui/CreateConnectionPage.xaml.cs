namespace Buform.Example.Maui;

public partial class CreateConnectionPage : ContentPage
{
    public CreateConnectionPage(CreateConnectionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
