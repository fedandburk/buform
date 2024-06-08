namespace Buform;

public partial class CreateConnectionPage : ContentPage
{
    public CreateConnectionPage(CreateConnectionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
