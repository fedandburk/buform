using System.Linq.Expressions;

namespace Buform;

public class SegmentsFormItem<TValue> : ValidatableFormItem<TValue>, ISegmentsFormItem
{
    private Func<TValue?, string?>? _formatter;
    private IEnumerable<TValue>? _source;

    IEnumerable<ISegmentsOptionFormItem> ISegmentsFormItem.Items => Items;

    public virtual string? Label
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual Func<TValue?, string?>? Formatter
    {
        get => _formatter;
        set
        {
            _formatter = value;

            foreach (var item in Items)
            {
                item.Formatter = Formatter;
            }

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(Items));
        }
    }

    public virtual IEnumerable<TValue>? Source
    {
        get => _source;
        set
        {
            _source = value;

            DisposeItems();

            Items = _source?.Select(CreateItem).ToList() ?? [];

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(Items));
        }
    }

    public virtual IReadOnlyList<SegmentsOptionFormItem<TValue>> Items { get; private set; }

    public SegmentsFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        Items = [];
    }

    protected virtual SegmentsOptionFormItem<TValue> CreateItem(TValue value)
    {
        return new SegmentsOptionFormItem<TValue>(value) { Formatter = Formatter };
    }

    private void DisposeItems()
    {
        foreach (var item in Items)
        {
            item.Dispose();
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            DisposeItems();

            _formatter = null;
            _source = null;
        }

        base.Dispose(isDisposing);
    }
}

public class SegmentsFormItem : SegmentsFormItem<string?>
{
    public SegmentsFormItem(Expression<Func<string?>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }
}
