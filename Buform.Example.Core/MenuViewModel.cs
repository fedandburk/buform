using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Buform;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class MenuViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Form _form;

    public MenuViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        Form = new Form(this)
        {
            new LogoFormGroup(),
            new TextFormGroup("Gallery")
            {
                new ButtonFormItem(ShowControlsCommand)
                {
                    Label = "Show All Components",
                    InputType = ButtonInputType.Done
                }
            },
            new TextFormGroup("Examples", "Contains some real-life examples.")
            {
                new ButtonFormItem(CreateConnectionCommand)
                {
                    Label = "Setup New Connection",
                    InputType = ButtonInputType.Done
                },
                new ButtonFormItem(CreateEventCommand)
                {
                    Label = "Create New Event",
                    InputType = ButtonInputType.Done
                }
            }
        };
    }

    [RelayCommand]
    private Task CreateConnectionAsync()
    {
        return _navigationService.OpenCreateConnectionAsync();
    }

    [RelayCommand]
    private Task CreateEventAsync(CancellationToken cancellationToken)
    {
        return _navigationService.OpenCreateEventAsync();
    }

    [RelayCommand]
    private Task ShowControlsAsync()
    {
        return _navigationService.OpenComponentsAsync();
    }
}
