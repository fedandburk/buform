namespace Buform.Example.Maui;

public partial class CreateEventPage : ContentPage
{
    public CreateEventPage(CreateEventViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
