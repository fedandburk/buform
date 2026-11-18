using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using DynamicData;
using DynamicData.Binding;

namespace Buform;

public class Form : ObservableCollection<IFormGroup>, IDisposable
{
    private readonly ReadOnlyObservableCollection<IFormItem> _items;
    private readonly CompositeDisposable _subscriptions;

    private bool _isReadOnly;
    private bool _isDisposed;

    public virtual IFormItem? this[Expression<Func<object?>> property] => GetItem(property);

    public virtual IFormItem? this[string propertyName] => GetItem(propertyName);

    protected virtual object? Target { get; private set; }

    public event EventHandler<FormValueChangedEventArgs>? ValueChanged;

    public virtual bool IsReadOnly
    {
        get => _isReadOnly;
        set
        {
            if (_isReadOnly == value)
            {
                return;
            }

            _isReadOnly = value;

            NotifyPropertyChanged();

            foreach (var item in _items)
            {
                item.IsReadOnly = value;
            }
        }
    }

    public Form(object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        Target = target;

        var items = this.ToObservableChangeSet()
            .MergeManyChangeSets(group => group.Connect())
            .OnItemAdded(InitializeItem)
            .Publish();

        var itemsSubscription = items.Bind(out _items).Subscribe();

        var valueChangedSubscription = items
            .MergeMany(item =>
                Observable.FromEventPattern<FormValueChangedEventArgs>(
                    handler => item.ValueChanged += handler,
                    handler => item.ValueChanged -= handler
                )
            )
            .Subscribe(pattern => OnItemValueChanged(pattern.Sender, pattern.EventArgs));

        _subscriptions = new CompositeDisposable(
            itemsSubscription,
            valueChangedSubscription,
            items.Connect()
        );
    }

    protected virtual void OnItemValueChanged(object? sender, FormValueChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    private void InitializeItem(IFormItem item)
    {
        if (Target != null)
        {
            item.Initialize(this, Target);
        }

        item.IsReadOnly = IsReadOnly;
    }

    public virtual TItem? GetItem<TItem>(Expression<Func<object?>> property)
        where TItem : class, IFormItem
    {
        return GetItem<TItem>(property.GetMemberName());
    }

    public virtual IFormItem? GetItem(Expression<Func<object?>> property)
    {
        return GetItem(property.GetMemberName());
    }

    public virtual TItem? GetItem<TItem>(string propertyName)
        where TItem : class, IFormItem
    {
        return GetItem(propertyName) as TItem;
    }

    public virtual IFormItem? GetItem(string propertyName)
    {
        return _items.FirstOrDefault(item => item.PropertyName == propertyName);
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

        _subscriptions.Dispose();

        foreach (var group in this)
        {
            group.Dispose();
        }

        Target = null;
    }
}
