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
    private ObservableCollection<int> _list2 = new(Enumerable.Range(1, 10));

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
            new ListFormGroup<int, TextFormItem<int>>(
                item => new TextFormItem<int>(item) { Formatter = i => i.ToWords() },
                "List"
            )
            {
            Source = List2,
            RemoveCommand = RemoveListItemCommand,
            MoveCommand = MoveListItemCommand,
            SelectCommand = SelectListItemCommand
        }                                        
        };
    }

    [RelayCommand]
    private void ToggleReadOnlyMode()
    {
        if (List2.Count > 3)
        {
            List.Add(1);
            List2.RemoveAt(2);
        }
        else
        {
            List2.Add(1);
            List.RemoveAt(2);
        }
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