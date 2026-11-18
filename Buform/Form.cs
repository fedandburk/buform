using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using DynamicData;
using DynamicData.Binding;

namespace Buform;

public class Form : ObservableCollection<IFormGroup>, IDisposable
{
    private readonly ReadOnlyObservableCollection<IFormItem> _items;
    private readonly CompositeDisposable _subscriptions;

    private bool _isDisposed;

    public virtual IFormItem? this[Expression<Func<object?>> property] => GetItem(property);

    public virtual IFormItem? this[string propertyName] => GetItem(propertyName);

    protected virtual object? Target { get; private set; }

    protected IReadOnlyList<IFormItem> AllItems => _items;

    public event EventHandler<FormValueChangedEventArgs>? ValueChanged;

    public virtual bool IsReadOnly
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

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

        var itemsSubscription = this.ToObservableChangeSet()
            .MergeManyChangeSets(group => group.Connect())
            .Bind(out _items)
            .Subscribe();

        var groupsSubscription = this.ToObservableChangeSet()
            .SubscribeMany(group => group.Connect().SubscribeMany(SubscribeItem).Subscribe())
            .Subscribe();

        _subscriptions = new CompositeDisposable(itemsSubscription, groupsSubscription);
    }

    private IDisposable SubscribeItem(IFormItem item)
    {
        item.ValueChanged += OnItemValueChanged;

        InitializeItem(item);

        return Disposable.Create(() => item.ValueChanged -= OnItemValueChanged);
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
