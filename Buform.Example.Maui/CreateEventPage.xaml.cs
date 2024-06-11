namespace Buform;

public partial class CreateEventPage : ContentPage
{
    public CreateEventPage(CreateEventViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
