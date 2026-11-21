// ReSharper disable ClassNeverInstantiated.Global

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using NLipsum.Core;

namespace Buform.Example;

public partial class ComponentsViewModel : ObservableObject
{
    public enum Enum
    {
        First,
        Second,
        Third,
    }

    [ObservableProperty]
    public partial Color Color { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> List { get; set; }

    [ObservableProperty]
    public partial Enum Segments { get; set; }

    [ObservableProperty]
    public partial string? Text { get; set; }

    [ObservableProperty]
    public partial string? MultilineText { get; set; }

    [ObservableProperty]
    public partial float Slider { get; set; }

    [ObservableProperty]
    public partial int Stepper { get; set; }

    [ObservableProperty]
    public partial DateTime DateTime { get; set; }

    [ObservableProperty]
    public partial bool Switch { get; set; }

    [ObservableProperty]
    public partial string? HiddenText { get; set; }

    [ObservableProperty]
    public partial string? CallbackPicker { get; set; }

    [ObservableProperty]
    public partial int? Picker { get; set; }

    [ObservableProperty]
    public partial int? AsyncPicker { get; set; }

    [ObservableProperty]
    public partial int[]? MultiValuePicker { get; set; }

    [ObservableProperty]
    public partial int RandomNumber { get; set; }

    [ObservableProperty]
    public partial Form Form { get; set; }

    public ComponentsViewModel()
    {
        Color = Color.Gold;
        List = new ObservableCollection<int>(Enumerable.Range(1, 10));
        MultilineText = new LipsumGenerator().GenerateLipsum(1);
        DateTime = DateTime.UtcNow;

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
                SelectCommand = SelectListItemCommand,
            },
            new LabeledFormGroup("Pickers")
            {
                new MultiValuePickerFormItem<int>(() => MultiValuePicker)
                {
                    Label = "Multi-value Picker",
                    Message = "Please select a number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.Default,
                    OptionFormatter = item => item.ToWords(),
                    OptionFilterValueFactory = option => option.ToWords(),
                    ValueFormatter = value => value?.Humanize() ?? "None",
                    Source = Enumerable.Range(1, 10).ToArray(),
                },
                new MultiValuePickerFormItem<int>(() => MultiValuePicker)
                {
                    Label = "Dialog Multi-value Picker",
                    Message = "Please select a roman number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.Dialog,
                    OptionFormatter = item => item.ToRoman(),
                    OptionFilterValueFactory = option => option.ToRoman(),
                    ValueFormatter = value => value?.Humanize() ?? "None",
                    Source = Enumerable.Range(1, 10).ToArray(),
                },
                new CallbackPickerFormItem(() => CallbackPicker)
                {
                    Label = "Callback Picker",
                    Formatter = item => item ?? "None",
                    Callback = () => Task.FromResult(new LipsumGenerator().GenerateWords(1)[0])!,
                },
                new PickerFormItem<int?>(() => Picker)
                {
                    Label = "Picker",
                    Message = "Please select a number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.Default,
                    Formatter = item => item?.ToWords() ?? "None",
                    OptionFilterValueFactory = option => option?.ToWords() ?? "None",
                    Source = Enumerable.Range(1, 10).OfType<int?>().ToArray(),
                },
                new PickerFormItem<int?>(() => Picker)
                {
                    Label = "Dialog Picker",
                    Message = "Please select a metric number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.Dialog,
                    Formatter = item => item?.ToMetric() ?? "None",
                    OptionFilterValueFactory = option => option?.ToMetric() ?? "None",
                    Source = Enumerable.Range(1, 10).OfType<int?>().ToArray(),
                },
                new PickerFormItem<int?>(() => Picker)
                {
                    Label = "Pop-up Picker",
                    Message = "Please select a roman number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.PopUp,
                    Formatter = item => item?.ToRoman() ?? "None",
                    Source = Enumerable.Range(1, 10).OfType<int?>().ToArray(),
                },
                new AsyncPickerFormItem<int?>(() => AsyncPicker)
                {
                    Label = "Async Picker",
                    Message = "Please select a number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.Default,
                    Formatter = item => item?.ToWords() ?? "None",
                    OptionFilterValueFactory = option => option?.ToWords(),
                    SourceFactory = async cancellationToken =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken)
                            .ConfigureAwait(false);
                        return Enumerable.Range(1, 10).OfType<int?>().ToArray();
                    },
                },
                new AsyncPickerFormItem<int?>(() => AsyncPicker)
                {
                    Label = "Dialog Async Picker",
                    Message = "Please select a roman number.",
                    IsClearable = true,
                    Presentation = PickerPresentation.Dialog,
                    Formatter = item => item?.ToRoman() ?? "None",
                    OptionFilterValueFactory = option => option?.ToRoman() ?? "None",
                    SourceFactory = async cancellationToken =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken)
                            .ConfigureAwait(false);
                        return Enumerable.Range(1, 10).OfType<int?>().ToArray();
                    },
                },
                new ColorPickerFormItem(() => Color) { Label = "Color Picker" },
            },
            new LabeledFormGroup("Buttons")
            {
                new ButtonFormItem(WriteLineCommand)
                {
                    Label = "Default Button",
                    Style = ButtonStyle.Default,
                },
                new ButtonFormItem(WriteLineCommand)
                {
                    Label = "Destructive Button",
                    Style = ButtonStyle.Destructive,
                },
                new ButtonFormItem(WriteLineCommand)
                {
                    Label = "Done Button",
                    Style = ButtonStyle.Done,
                },
            },
            new LabeledFormGroup("Segments")
            {
                new SegmentsFormItem<Enum>(() => Segments)
                {
                    Label = "Segments",
                    Source = System.Enum.GetValues(typeof(Enum)).OfType<Enum>(),
                },
            },
            new LabeledFormGroup("Texts")
            {
                new TextInputFormItem(() => Text)
                {
                    Placeholder = "Default text",
                    InputType = TextInputType.Default,
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Number & Punctuation Text",
                    InputType = TextInputType.NumberAndPunctuation,
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Number Text",
                    InputType = TextInputType.Number,
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Decimal Text",
                    InputType = TextInputType.Decimal,
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Phone Text",
                    InputType = TextInputType.Phone,
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Url Text",
                    InputType = TextInputType.Url,
                },
                new TextInputFormItem(() => Text)
                {
                    Label = "Email Address Text",
                    InputType = TextInputType.EmailAddress,
                },
                new TextInputFormItem(() => Text) { Label = "Password Text", IsSecure = true },
                new MultilineTextInputFormItem(() => MultilineText)
                {
                    Placeholder = "Multiline text",
                },
            },
            new LabeledFormGroup("Sliders")
            {
                new TextInputFormItem<float>(
                    () => Slider,
                    @string => float.TryParse(@string, out var value) ? value : 0
                )
                {
                    Label = "Slider value",
                    InputType = TextInputType.Decimal,
                },
                new SliderFormItem(() => Slider) { MinValue = 0, MaxValue = 10 },
            },
            new LabeledFormGroup("Switches")
            {
                new SwitchFormItem(() => Switch)
                {
                    Label = "Switch",
                    ValueChangedCallback = (form, value) =>
                        form.GetItem(() => HiddenText)!.IsVisible = value,
                },
                new TextInputFormItem(() => HiddenText)
                {
                    Label = "Hidden text",
                    IsVisible = false,
                },
            },
            new LabeledFormGroup("Steppers")
            {
                new TextInputFormItem<int>(
                    () => Stepper,
                    @string => int.TryParse(@string, out var value) ? value : 0
                )
                {
                    Label = "Stepper value",
                    InputType = TextInputType.Number,
                },
                new StepperFormItem(() => Stepper)
                {
                    Label = "Stepper",
                    Step = 5,
                    MinValue = -10,
                    MaxValue = 10,
                },
            },
            new LabeledFormGroup("Date & Time")
            {
                new DateTimeFormItem(() => DateTime)
                {
                    Label = "Date and time",
                    InputType = DateTimeInputType.DateTime,
                },
                new DateTimeFormItem(() => DateTime)
                {
                    Label = "Date",
                    InputType = DateTimeInputType.Date,
                },
                new DateTimeFormItem(() => DateTime)
                {
                    Label = "Time",
                    InputType = DateTimeInputType.Time,
                },
            },
            new LabeledFormGroup("Custom views & items", "Demonstrates custom items and item views")
            {
                new RandomNumberGeneratorItem(() => RandomNumber),
            },
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
    private void MoveListItem(FormItemMove move)
    {
        List.Move(move.OldIndex, move.NewIndex);
    }
}
