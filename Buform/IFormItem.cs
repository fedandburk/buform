using System.ComponentModel;

namespace Buform;

public interface IFormItem : INotifyPropertyChanged, IDisposable
{
    event EventHandler<FormValueChangedEventArgs>? ValueChanged;

    string? PropertyName { get; }

    bool IsReadOnly { get; set; }

    bool IsVisible { get; set; }

    object? Value { get; set; }

    void Initialize(Form form, object target);
}
