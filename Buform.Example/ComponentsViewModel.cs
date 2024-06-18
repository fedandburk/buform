using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using NLipsum.Core;

namespace Buform;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class ComponentsViewModel : ObservableObject
{
    public enum Enum
    {
        First,
        Second,
        Third
    }

    [ObservableProperty]
    private Color _color = Color.Gold;

    [ObservableProperty]
    private ObservableCollection<int> _list = new(Enumerable.Range(1, 10));

    [ObservableProperty]
    private Enum _segments;

    [ObservableProperty]
    private string? _text;

    [ObservableProperty]
    private string? _multilineText = new LipsumGenerator().GenerateLipsum(1);

    [ObservableProperty]
    private float _slider;

    [ObservableProperty]
    private int _stepper;

    [ObservableProperty]
    private DateTime _dateTime = DateTime.UtcNow;

    [ObservableProperty]
    private bool _switch;

    [ObservableProperty]
    private string? _hiddenText;

    [ObservableProperty]
    private string? _callbackPicker;

    [ObservableProperty]
    private int? _picker;

    [ObservableProperty]
    private int? _asyncPicker;

    [ObservableProperty]
    private int[]? _multiValuePicker;

    [ObservableProperty]
    private int _randomNumber;

    [ObservableProperty]
    private Form _form;

    public ComponentsViewModel()
    {
        Form = new Form(this)
        {
            new ListFormGroup<int, TextFormItem<int>>(
                item => new TextFormItem<int>(item) { Formatter = i => i.ToWords() },
                "List"
            )
            {
                Source = List,
                RemoveCommand = RemoveListItemCommand,
                MoveCommand = MoveListItemCommand,
                SelectCommand = SelectListItemCommand
            },
            new TextFormGroup("Pickers")
            {
                new MultiValuePickerFormItem<int>(() => MultiValuePicker)
                {
                    Label = "Multi-value Picker",
                    Message = "Please select a number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.Default,
                    ItemFormatter = item => item.ToWords(),
                    OptionsFilterValueFactory = option => option.ToWords(),
                    ValueFormatter = value => value?.Humanize() ?? "None",
                    Source = Enumerable.Range(1, 10).ToArray()
                },
                new MultiValuePickerFormItem<int>(() => MultiValuePicker)
                {
                    Label = "Dialog Multi-value Picker",
                    Message = "Please select a roman number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.Dialog,
                    ItemFormatter = item => item.ToRoman(),
                    OptionsFilterValueFactory = option => option.ToRoman(),
                    ValueFormatter = value => value?.Humanize() ?? "None",
                    Source = Enumerable.Range(1, 10).ToArray()
                },
                new CallbackPickerFormItem(() => CallbackPicker)
                {
                    Label = "Callback Picker",
                    Formatter = item => item ?? "None",
                    Callback = () => Task.FromResult(new LipsumGenerator().GenerateWords(1)[0])!
                },
                new PickerFormItem<int?>(() => Picker)
                {
                    Label = "Picker",
                    Message = "Please select a number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.Default,
                    Formatter = item => item?.ToWords() ?? "None",
                    OptionsFilterValueFactory = option => option?.ToWords() ?? "None",
                    Source = Enumerable.Range(1, 10).OfType<int?>().ToArray()
                },
                new PickerFormItem<int?>(() => Picker)
                {
                    Label = "Dialog Picker",
                    Message = "Please select a metric number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.Dialog,
                    Formatter = item => item?.ToMetric() ?? "None",
                    OptionsFilterValueFactory = option => option?.ToMetric() ?? "None",
                    Source = Enumerable.Range(1, 10).OfType<int?>().ToArray()
                },
                new PickerFormItem<int?>(() => Picker)
                {
                    Label = "Pop-up Picker",
                    Message = "Please select a roman number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.PopUp,
                    Formatter = item => item?.ToRoman() ?? "None",
                    Source = Enumerable.Range(1, 10).OfType<int?>().ToArray()
                },
                new AsyncPickerFormItem<int?>(() => AsyncPicker)
                {
                    Label = "Async Picker",
                    Message = "Please select a number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.Default,
                    Formatter = item => item?.ToWords() ?? "None",
                    OptionsFilterValueFactory = option => option?.ToWords(),
                    SourceFactory = async cancellationToken =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken)
                            .ConfigureAwait(false);
                        return Enumerable.Range(1, 10).OfType<int?>().ToArray();
                    }
                },
                new AsyncPickerFormItem<int?>(() => AsyncPicker)
                {
                    Label = "Dialog Async Picker",
                    Message = "Please select a roman number.",
                    CanBeCleared = true,
                    InputType = PickerInputType.Dialog,
                    Formatter = item => item?.ToRoman() ?? "None",
                    OptionsFilterValueFactory = option => option?.ToRoman() ?? "None",
                    SourceFactory = async cancellationToken =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken)
                            .ConfigureAwait(false);
                        return Enumerable.Range(1, 10).OfType<int?>().ToArray();
                    }
                },
                new ColorPickerFormItem(() => Color) { Label = "Color Picker" }
            },
            new TextFormGroup("Buttons")
            {
                new ButtonFormItem(WriteLineCommand)
                {
                    Label = "Default Button",
                    InputType = ButtonInputType.Default
                },
                new ButtonFormItem(WriteLineCommand)
                {
                    Label = "Destructive Button",
                    InputType = ButtonInputType.Destructive
                },
                new ButtonFormItem(WriteLineCommand)
                {
                    Label = "Done Button",
                    InputType = ButtonInputType.Done
                }
            },
            new TextFormGroup("Segments")
            {
                new SegmentsFormItem<Enum>(() => Segments)
                {
                    Label = "Segments",
                    Source = System.Enum.GetValues(typeof(Enum)).OfType<Enum>()
                }
            },
            new TextFormGroup("Texts")
            {
                new TextInputFormItem(() => Text)
                {
                    Placeholder = "Default text",
                    InputType = TextInputType.Default
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Number & Punctuation Text",
                    InputType = TextInputType.NumberAndPunctuation
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Number Text",
                    InputType = TextInputType.Number
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Decimal Text",
                    InputType = TextInputType.Decimal
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Phone Text",
                    InputType = TextInputType.Phone
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Url Text",
                    InputType = TextInputType.Url
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Email Address Text",
                    InputType = TextInputType.EmailAddress
                },
                new TextInputFormItem(() => Text) { Label = "Password Text", IsSecured = true },
                new MultilineTextInputFormItem(() => MultilineText)
                {
                    Placeholder = "Multiline text"
                }
            },
            new TextFormGroup("Sliders")
            {
                new TextInputFormItem<float>(
                    () => Slider,
                    @string => float.TryParse(@string, out var value) ? value : 0
                )
                {
                    Label = "Slider value",
                    InputType = TextInputType.Decimal
                },
                new SliderFormItem(() => Slider) { MinValue = 0, MaxValue = 10 }
            },
            new TextFormGroup("Switches")
            {
                new SwitchFormItem(() => Switch)
                {
                    Label = "Switch",
                    ValueChangedCallback = (form, value) =>
                        form.GetItem(() => HiddenText)!.IsVisible = value
                },
                new TextInputFormItem(() => HiddenText) { Label = "Hidden text", IsVisible = false }
            },
            new TextFormGroup("Steppers")
            {
                new TextInputFormItem<int>(
                    () => Stepper,
                    @string => int.TryParse(@string, out var value) ? value : 0
                )
                {
                    Label = "Stepper value",
                    InputType = TextInputType.Number
                },
                new StepperFormItem(() => Stepper)
                {
                    Label = "Stepper",
                    StepAmount = 5,
                    MinValue = -10,
                    MaxValue = 10
                }
            },
            new TextFormGroup("Date & Time")
            {
                new DateTimeFormItem(() => DateTime)
                {
                    Label = "Date and time",
                    InputType = DateTimeInputType.DateTime
                },
                new DateTimeFormItem(() => DateTime)
                {
                    Label = "Date",
                    InputType = DateTimeInputType.Date
                },
                new DateTimeFormItem(() => DateTime)
                {
                    Label = "Time",
                    InputType = DateTimeInputType.Time
                }
            },
            new TextFormGroup("Custom views & items", "Demonstrates custom items and item views")
            {
                new RandomNumberGeneratorItem(() => RandomNumber)
            },
            new TextFormGroup("MvvmCross", "Demonstrates MvvmCross bindings in item views")
            {
                // new MvxButtonFormItem(WriteLineCommand) { Label = "MvvmCross Button" }
            }
        };
    }

    [RelayCommand]
    private void ToggleReadOnlyMode()
    {
        Form.IsReadOnly = !Form.IsReadOnly;
    }

    [RelayCommand]
    private static void WriteLine()
    {
        Console.WriteLine("Command executed");
    }

    [RelayCommand]
    private static void SelectListItem(int item)
    {
        Console.WriteLine($"Selected item: {item}");
    }

    [RelayCommand]
    private void RemoveListItem(int item)
    {
        List.Remove(item);
    }

    [RelayCommand]
    private void MoveListItem((int oldIndex, int newIndex) move)
    {
        List.Move(move.oldIndex, move.newIndex);
    }

    protected override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);

        if (e.PropertyName == null)
        {
            return;
        }

        var value = GetType().GetProperty(e.PropertyName!)?.GetValue(this);

        Console.WriteLine($"{e.PropertyName} changing: {value}");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == null)
        {
            return;
        }

        var value = GetType().GetProperty(e.PropertyName!)?.GetValue(this);

        Console.WriteLine($"{e.PropertyName} changed: {value}");
    }
}
