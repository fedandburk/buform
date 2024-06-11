using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Humanizer;

namespace Buform;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class CreateEventViewModel : ObservableObject
{
    public enum RepeatType
    {
        Never,
        EveryDay,
        EveryWeek,
        Every2Weeks,
        EveryMonth,
        EveryYear
    }

    public enum TravelTimeType
    {
        None,
        FiveMinutes,
        FifteenMinutes,
        ThirtyMinutes,
        OneHour,
        OneHourThirtyMinutes,
        TwoHours
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class Model
    {
        public string? Title { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime? StartsAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndsAt { get; set; } = DateTime.UtcNow + TimeSpan.FromHours(1);
        public RepeatType Repeat { get; set; }
        public TravelTimeType TravelTime { get; set; }
        public string? Url { get; set; }
        public string? Notes { get; set; }
    }

    public class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            RuleFor(item => item.Title).NotEmpty();
        }
    }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private FluentValidationForm<Model> _form;

    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    private bool CanCreate => Form?.IsValid ?? false;

    public CreateEventViewModel()
    {
        var model = new Model();

        Form = new FluentValidationForm<Model>(model, new Validator())
        {
            new TextFormGroup
            {
                new TextInputFormItem(() => model.Title) { Placeholder = "Title" },
                new TextInputFormItem(() => model.Location)
                {
                    Placeholder = "Location or Video Call"
                }
            },
            new TextFormGroup
            {
                new SwitchFormItem(() => model.IsAllDay)
                {
                    Label = "All-day",
                    ValueChangedCallback = (form, value) =>
                    {
                        var inputType = value ? DateTimeInputType.Date : DateTimeInputType.DateTime;

                        form.GetItem<DateTimeFormItem>(() => model.StartsAt)!.InputType = inputType;
                        form.GetItem<DateTimeFormItem>(() => model.EndsAt)!.InputType = inputType;

                        form.GetItem(() => model.TravelTime)!.IsVisible = !value;
                    }
                },
                new DateTimeFormItem(() => model.StartsAt)
                {
                    Label = "Starts",
                    InputType = DateTimeInputType.DateTime
                },
                new DateTimeFormItem(() => model.EndsAt)
                {
                    Label = "Ends",
                    InputType = DateTimeInputType.DateTime
                },
                new PickerFormItem<RepeatType>(() => model.Repeat)
                {
                    Label = "Repeat",
                    Source = Enum.GetValues(typeof(RepeatType)).OfType<RepeatType>(),
                    Formatter = item => item.Humanize(LetterCasing.Title)
                },
                new PickerFormItem<TravelTimeType>(() => model.TravelTime)
                {
                    Label = "Travel Time",
                    Source = Enum.GetValues(typeof(TravelTimeType)).OfType<TravelTimeType>(),
                    Formatter = item => item.Humanize(LetterCasing.Title)
                }
            },
            new TextFormGroup
            {
                new ButtonFormItem(AddAttachmentCommand) { Label = "Add attachment..." }
            },
            new TextFormGroup
            {
                new TextInputFormItem(() => model.Url)
                {
                    Placeholder = "URL",
                    InputType = TextInputType.Url
                },
                new MultilineTextInputFormItem(() => model.Notes) { Placeholder = "Notes" }
            },
        };
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private static void Create()
    {
        Console.WriteLine("Create command executed");
    }

    [RelayCommand]
    private static void AddAttachment()
    {
        Console.WriteLine("AddAttachment command executed");
    }
}
