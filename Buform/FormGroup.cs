using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;

namespace Buform;

public abstract class FormGroup<TFormItem> : IFormGroup
    where TFormItem : IFormItem
{
    private readonly SourceList<TFormItem> _items;
    private readonly ReadOnlyObservableCollection<TFormItem> _visibleItems;
    private readonly CompositeDisposable _subscriptions;

    private ICommand? _removeCommand;
    private ICommand? _moveCommand;
    private ICommand? _insertCommand;
    private bool _isDisposed;

    public int Count => _visibleItems.Count;

    public TFormItem this[int index] => _visibleItems[index];

    public virtual ICommand? RemoveCommand
    {
        get => _removeCommand;
        set
        {
            _removeCommand = value;
            NotifyPropertyChanged();
        }
    }

    public virtual ICommand? MoveCommand
    {
        get => _moveCommand;
        set
        {
            _moveCommand = value;
            NotifyPropertyChanged();
        }
    }

    public virtual ICommand? InsertCommand
    {
        get => _insertCommand;
        set
        {
            _insertCommand = value;
            NotifyPropertyChanged();
        }
    }

    protected ISourceList<TFormItem> Items => _items;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected FormGroup()
    {
        _items = new SourceList<TFormItem>();

        var visibleItemsSubscription = _items
            .Connect()
            .DisposeMany()
            .AutoRefresh(item => item.IsVisible)
            .Filter(item => item.IsVisible)
            .Bind(out _visibleItems)
            .Subscribe();

        _subscriptions = new CompositeDisposable(visibleItemsSubscription, _items);

        ((INotifyCollectionChanged)_visibleItems).CollectionChanged += OnVisibleItemsChanged;
        ((INotifyPropertyChanged)_visibleItems).PropertyChanged += OnVisibleItemsPropertyChanged;
    }

    private void OnVisibleItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    private void OnVisibleItemsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public IObservable<IChangeSet<IFormItem>> Connect()
    {
        return _items.Connect().Transform(item => (IFormItem)item);
    }

    public void Add(TFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }

    public void Insert(int index, TFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Insert(index, item);
    }

    public bool Remove(TFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _items.Remove(item);
    }

    public void Move(int originalIndex, int destinationIndex)
    {
        _items.Move(originalIndex, destinationIndex);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public IEnumerator<IFormItem> GetEnumerator()
    {
        return _visibleItems.Cast<IFormItem>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (!isDisposing)
        {
            return;
        }

        ((INotifyCollectionChanged)_visibleItems).CollectionChanged -= OnVisibleItemsChanged;
        ((INotifyPropertyChanged)_visibleItems).PropertyChanged -= OnVisibleItemsPropertyChanged;

        _subscriptions.Dispose();
    }
}
