using System.Collections.Specialized;

namespace Buform;

public class ListFormGroup<TValue, TItem> : FormGroup<TItem>, IListFormGroup
    where TItem : FormItem<TValue>
{
    private Func<TValue, TItem>? _itemFactory;
    private string? _headerLabel;
    private string? _footerLabel;
    private IEnumerable<TValue>? _source;

    public virtual string? HeaderLabel
    {
        get => _headerLabel;
        set
        {
            _headerLabel = value;

            NotifyPropertyChanged();
        }
    }

    public virtual string? FooterLabel
    {
        get => _footerLabel;
        set
        {
            _footerLabel = value;

            NotifyPropertyChanged();
        }
    }

    public virtual IEnumerable<TValue>? Source
    {
        get => _source;
        set
        {
            if (_source is INotifyCollectionChanged oldNotifyCollectionChanged)
            {
                oldNotifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
            }

            _source = value ?? Array.Empty<TValue>();

            if (_source is INotifyCollectionChanged newNotifyCollectionChanged)
            {
                newNotifyCollectionChanged.CollectionChanged += OnCollectionChanged;
            }

            Reset();

            NotifyPropertyChanged();
        }
    }

    public ListFormGroup(
        Func<TValue, TItem> itemFactory,
        string? headerLabel = null,
        string? footerLabel = null
    )
    {
        ArgumentNullException.ThrowIfNull(itemFactory);

        _itemFactory = itemFactory;
        _headerLabel = headerLabel;
        _footerLabel = footerLabel;
    }

    protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Reset();

        NotifyPropertyChanged(nameof(Source));
    }

    protected virtual void Reset()
    {
        if (_itemFactory == null)
        {
            return;
        }

        ClearItems();

        if (Source == null)
        {
            return;
        }

        foreach (var value in Source)
        {
            var item = _itemFactory(value);

            Add(item);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (_source is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
            }
        }

        _itemFactory = null;
        _source = null;

        base.Dispose(isDisposing);
    }
}
