// ReSharper disable ClassNeverInstantiated.Global

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Buform.Example;

public sealed partial class MenuViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public MenuViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        Form = new Form(this)
        {
            new LogoFormGroup(),
            new LabeledFormGroup("Gallery")
            {
                new ButtonFormItem(ShowControlsCommand)
                {
                    Label = "Show All Components",
                    Style = ButtonStyle.Done,
                },
            },
            new LabeledFormGroup("Examples", "Contains some real-life examples.")
            {
                new ButtonFormItem(CreateConnectionCommand)
                {
                    Label = "Setup New Connection",
                    Style = ButtonStyle.Done,
                },
                new ButtonFormItem(CreateEventCommand)
                {
                    Label = "Create New Event",
                    Style = ButtonStyle.Done,
                },
            },
        };
    }

    [ObservableProperty]
    public partial Form Form { get; set; }

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
