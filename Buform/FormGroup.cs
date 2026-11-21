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

    private bool _isDisposed;

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

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public int Count => _visibleItems.Count;

    public virtual ICommand? RemoveCommand
    {
        get;
        set
        {
            field = value;
            NotifyPropertyChanged();
        }
    }

    public virtual ICommand? MoveCommand
    {
        get;
        set
        {
            field = value;
            NotifyPropertyChanged();
        }
    }

    public virtual ICommand? InsertCommand
    {
        get;
        set
        {
            field = value;
            NotifyPropertyChanged();
        }
    }

    protected ISourceList<TFormItem> Items => _items;

    public TFormItem this[int index] => _visibleItems[index];

    IFormItem IReadOnlyList<IFormItem>.this[int index] => this[index];

    public IObservable<IChangeSet<IFormItem>> Connect()
    {
        return _items.Connect().Transform(item => (IFormItem)item);
    }

    public int GetSourceIndex(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _items.Items.Cast<IFormItem>().ToList().IndexOf(item);
    }

    public void Add(TFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }

    public void Insert(int index, TFormItem item)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _items.Count);
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
        ArgumentOutOfRangeException.ThrowIfNegative(originalIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(originalIndex, _items.Count);
        ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(destinationIndex, _items.Count);

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

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    private void OnVisibleItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    private void OnVisibleItemsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}
