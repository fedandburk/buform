using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;

namespace Buform;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class CreateConnectionViewModel : ObservableObject
{
    public class Validator : AbstractValidator<CreateConnectionViewModel>
    {
        public Validator()
        {
            RuleFor(item => item.Server).NotEmpty();
            RuleFor(item => item.Port).NotEmpty().GreaterThan(0);
            RuleFor(item => item.Password).NotEmpty();
        }
    }

    public enum ConnectionType
    {
        Ftp,
        FtpSsl,
        WebDav
    }

    private const string AnonymousUsername = "anonymous";

    [ObservableProperty]
    private string? _server;

    [ObservableProperty]
    private int? _port;

    [ObservableProperty]
    private ConnectionType _type;

    [ObservableProperty]
    private bool _isAnonymousLogin;

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _password;

    [ObservableProperty]
    private string? _sshPrivateKey;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private FluentValidationForm<CreateConnectionViewModel> _form;

    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    private bool CanConnect => Form?.IsValid ?? false;

    public CreateConnectionViewModel()
    {
        Reset();

        Form = new FluentValidationForm<CreateConnectionViewModel>(this, new Validator())
        {
            new TextFormGroup("Server")
            {
                new TextInputFormItem(() => Server)
                {
                    Placeholder = "Server",
                    InputType = TextInputType.Url
                },
                new TextInputFormItem<int?>(
                    () => Port,
                    @string => int.TryParse(@string, out var port) ? port : null
                )
                {
                    Label = "Port",
                    Placeholder = "21",
                    InputType = TextInputType.Number
                },
                new SegmentsFormItem<ConnectionType>(() => Type)
                {
                    Formatter = FormatConnectionType,
                    Source = new[]
                    {
                        ConnectionType.Ftp,
                        ConnectionType.FtpSsl,
                        ConnectionType.WebDav
                    }
                }
            },
            new TextFormGroup(
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
                    }
                },
                new TextInputFormItem(() => Username)
                {
                    Label = "Username",
                    Placeholder = "Username",
                    InputType = TextInputType.EmailAddress
                },
                new TextInputFormItem(() => Password)
                {
                    Label = "Password",
                    Placeholder = "Password",
                    IsSecured = true
                }
            },
            new TextFormGroup
            {
                new PickerFormItem(() => SshPrivateKey)
                {
                    Label = "SSH Private Key",
                    InputType = PickerInputType.PopUp,
                    Source = new[] { "Key 1", "Key 2", "Key 3" }
                }
            },
            new TextFormGroup
            {
                new ButtonFormItem(ConnectCommand)
                {
                    Label = "Connect",
                    InputType = ButtonInputType.Done
                }
            },
            new TextFormGroup
            {
                new ButtonFormItem(ResetCommand)
                {
                    Label = "Reset",
                    InputType = ButtonInputType.Destructive
                }
            }
        };
    }

    private static string FormatConnectionType(ConnectionType value)
    {
        return value switch
        {
            ConnectionType.Ftp => "FTP",
            ConnectionType.FtpSsl => "FTP-SSL",
            ConnectionType.WebDav => "WebDAW",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private static void Connect()
    {
        Console.WriteLine("Connect command executed");
    }

    [RelayCommand]
    private void Reset()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Form?.ResetValidation();

        Server = null;
        Port = 21;
        Type = ConnectionType.Ftp;
        IsAnonymousLogin = false;
        Username = null;
        Password = null;
        SshPrivateKey = null;
    }
}
