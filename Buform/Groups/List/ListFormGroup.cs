using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;

namespace Buform;

public class ListFormGroup<TValue, TItem> : FormGroup<TItem>, IListFormGroup
    where TValue : notnull
    where TItem : FormItem<TValue>
{
    private readonly Func<TValue, TItem> _itemFactory;
    private string? _headerLabel;
    private string? _footerLabel;
    private IDisposable? _sourceSubscription;

    public virtual ICommand? SelectCommand
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

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
        get;
        set
        {
            _sourceSubscription?.Dispose();

            Items.Clear();

            field = value;

            _sourceSubscription = ToChangeSet(field ?? [])
                .Transform(_itemFactory)
                .PopulateInto(Items);

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

    private static IObservable<IChangeSet<TValue>> ToChangeSet(IEnumerable<TValue> source)
    {
        return source switch
        {
            ObservableCollection<TValue> collection => collection.ToObservableChangeSet(),
            ReadOnlyObservableCollection<TValue> collection => collection.ToObservableChangeSet(),
            _ => source.AsObservableChangeSet(),
        };
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _sourceSubscription?.Dispose();
        }

        base.Dispose(isDisposing);
    }
}
