using System.ComponentModel;

namespace Buform.Tests;

public sealed class Model : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Name
    {
        get;
        set
        {
            field = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }
}
