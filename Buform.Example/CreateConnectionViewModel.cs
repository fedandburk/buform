// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;

namespace Buform.Example;

public partial class CreateConnectionViewModel : ObservableObject
{
    private const string AnonymousUsername = "anonymous";

    public CreateConnectionViewModel()
    {
        Reset();

        Form = new FluentValidationForm<CreateConnectionViewModel>(this, new Validator())
        {
            new LabeledFormGroup("Server")
            {
                new TextInputFormItem(() => Server)
                {
                    Placeholder = "Server",
                    InputType = TextInputType.Url,
                },
                new TextInputFormItem<int?>(
                    () => Port,
                    @string => int.TryParse(@string, out var port) ? port : null
                )
                {
                    Label = "Port",
                    Placeholder = "21",
                    InputType = TextInputType.Number,
                },
                new SegmentsFormItem<ConnectionType>(() => Type)
                {
                    Formatter = FormatConnectionType,
                    Source = new[]
                    {
                        ConnectionType.Ftp,
                        ConnectionType.FtpSsl,
                        ConnectionType.WebDav,
                    },
                },
            },
            new LabeledFormGroup(
                "Authorization",
                "Anonymous FTP logins are usually the username 'anonymous' with the user's email address as the password."
            )
            {
                new SwitchFormItem(() => IsAnonymousLogin)
                {
                    Label = "Anonymous login",
                    ValueChangedCallback = (form, value) =>
                    {
                        form[() => Username]!.IsReadOnly = value;
                        Username = value ? AnonymousUsername : null;
                        Password = null;
                    },
                },
                new TextInputFormItem(() => Username)
                {
                    Label = "Username",
                    Placeholder = "Username",
                    InputType = TextInputType.EmailAddress,
                },
                new TextInputFormItem(() => Password)
                {
                    Label = "Password",
                    Placeholder = "Password",
                    IsSecure = true,
                },
            },
            new LabeledFormGroup
            {
                new PickerFormItem(() => SshPrivateKey)
                {
                    Label = "SSH Private Key",
                    Presentation = PickerPresentation.PopUp,
                    Source = new[] { "Key 1", "Key 2", "Key 3" },
                },
            },
            new LabeledFormGroup
            {
                new ButtonFormItem(ConnectCommand) { Label = "Connect", Style = ButtonStyle.Done },
            },
            new LabeledFormGroup
            {
                new ButtonFormItem(ResetCommand)
                {
                    Label = "Reset",
                    Style = ButtonStyle.Destructive,
                },
            },
        };

        (Form as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
    }

    public enum ConnectionType
    {
        Ftp,
        FtpSsl,
        WebDav,
    }

    [ObservableProperty]
    public partial string? Server { get; set; }

    [ObservableProperty]
    public partial int? Port { get; set; }

    [ObservableProperty]
    public partial ConnectionType Type { get; set; }

    [ObservableProperty]
    public partial bool IsAnonymousLogin { get; set; }

    [ObservableProperty]
    public partial string? Username { get; set; }

    [ObservableProperty]
    public partial string? Password { get; set; }

    [ObservableProperty]
    public partial string? SshPrivateKey { get; set; }

    [ObservableProperty]
    public partial FluentValidationForm<CreateConnectionViewModel> Form { get; set; }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    public partial bool CanConnect { get; set; }

    private static string FormatConnectionType(ConnectionType value)
    {
        return value switch
        {
            ConnectionType.Ftp => "FTP",
            ConnectionType.FtpSsl => "FTP-SSL",
            ConnectionType.WebDav => "WebDAW",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private static void Connect()
    {
        Console.WriteLine("Connect command executed");
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CanConnect = Form.IsValid;
    }

    [RelayCommand]
    private void Reset()
    {
        Form?.ResetValidation();

        Server = null;
        Port = 21;
        Type = ConnectionType.Ftp;
        IsAnonymousLogin = false;
        Username = null;
        Password = null;
        SshPrivateKey = null;
    }

    public class Validator : AbstractValidator<CreateConnectionViewModel>
    {
        public Validator()
        {
            RuleFor(item => item.Server).NotEmpty();
            RuleFor(item => item.Port).NotEmpty().GreaterThan(0);
            RuleFor(item => item.Password).NotEmpty();
        }
    }
}
