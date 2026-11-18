using System.ComponentModel;

namespace Buform.Tests;

public sealed class Model : INotifyPropertyChanged
{
    private string? _name;

    public string? Name
    {
        get => _name;
        set
        {
            _name = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
