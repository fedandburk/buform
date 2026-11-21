using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using DynamicData;

namespace Buform;

public interface IFormGroup
    : IReadOnlyList<IFormItem>,
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        IDisposable
{
    ICommand? RemoveCommand { get; }
    ICommand? MoveCommand { get; }
    ICommand? InsertCommand { get; }

    IObservable<IChangeSet<IFormItem>> Connect();

    int GetSourceIndex(IFormItem item);
}
