using System.Collections.Specialized;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Fedandburk.Common.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class FormRecyclerViewAdapter : RecyclerView.Adapter
{
    private Form? _form;
    private IEnumerable<IFormItem>? _items;
    private IDisposable? _formSubscription;
    private IDisposable? _itemsSubscription;

    public Form? Form
    {
        get => _form;
        set
        {
            if (ReferenceEquals(Form, value))
            {
                return;
            }

            _formSubscription?.Dispose();
            _itemsSubscription?.Dispose();

            if (_items is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _items = null;

            _form = value;

            if (_form == null)
            {
                return;
            }

            _formSubscription = _form.WeakSubscribe(OnFormChanged);

            _items = _form.ObservableFlatten();

            if (_items is INotifyCollectionChanged notifyCollectionChanged)
            {
                _itemsSubscription = notifyCollectionChanged.WeakSubscribe(OnItemsChanged);
            }

            // TODO: Use Looper.MainLooper.
            NotifyDataSetChanged();
        }
    }

    public override int ItemCount => GetItemsCount();

    private void OnFormChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (_form == null || _formSubscription == null)
        {
            return;
        }

        // TODO: Use Looper.MainLooper.
        NotifyDataSetChanged();
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (_items == null || _itemsSubscription == null)
        {
            return;
        }

        // TODO: Use Looper.MainLooper.
        UpdateItems(args);
    }

    private void UpdateItems(NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add when args.NewItems != null:
                NotifyItemRangeInserted(args.NewStartingIndex, args.NewItems.Count);
                break;
            case NotifyCollectionChangedAction.Remove when args.OldItems != null:
                NotifyItemRangeRemoved(args.OldStartingIndex, args.OldItems.Count);
                break;
            case NotifyCollectionChangedAction.Replace when args.NewItems != null:
                NotifyItemRangeChanged(args.NewStartingIndex, args.NewItems.Count);
                break;
            case NotifyCollectionChangedAction.Move when args.NewItems != null:
                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    NotifyItemMoved(args.OldStartingIndex + i, args.NewStartingIndex + i);
                }

                break;
            case NotifyCollectionChangedAction.Reset:
                NotifyDataSetChanged();
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(NotifyCollectionChangedEventArgs),
                    args,
                    null
                );
        }
    }

    private int GetItemsCount()
    {
        if (_items == null || _form == null)
        {
            return 0;
        }

        var itemsCount = _items.Count();
        var sectionsCount = _form.Count * 2; // Each section has a header and a footer.

        return itemsCount + sectionsCount;
    }

    private bool TryGetData(
        int position,
        out IDisposable? data,
        out FormViewHolderType? viewHolderType
    )
    {
        if (_form == null)
        {
            data = null;
            viewHolderType = null;

            return false;
        }

        var index = 0;

        foreach (var section in _form)
        {
            if (index == position)
            {
                data = section;
                viewHolderType = FormViewHolderType.Header;

                return true;
            }

            index++; // Count the header.

            foreach (var item in section)
            {
                // ReSharper disable once InvertIf
                if (index == position)
                {
                    data = item;
                    viewHolderType = FormViewHolderType.Item;

                    return true;
                }

                index++; // Count the item.
            }

            // ReSharper disable once InvertIf
            if (index == position)
            {
                data = section;
                viewHolderType = FormViewHolderType.Footer;

                return true;
            }

            index++; // Count the footer.
        }

        data = null;
        viewHolderType = null;

        return false;
    }

    public override int GetItemViewType(int position)
    {
        var dataResult = TryGetData(position, out var data, out var viewHolderType);

        if (!dataResult)
        {
            throw new FormViewNotFoundException(position);
        }

        var dataType = data!.GetType();

        int? viewType;

        var result = viewHolderType! switch
        {
            FormViewHolderType.Item => FormPlatform.TryGetItemViewType(dataType, out viewType),
            FormViewHolderType.Header
                => FormPlatform.TryGetGroupHeaderViewType(dataType, out viewType),
            FormViewHolderType.Footer
                => FormPlatform.TryGetGroupFooterViewType(dataType, out viewType),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!result)
        {
            throw new FormViewNotFoundException(data);
        }

        return viewType!.Value;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var result = FormPlatform.TryGetViewHolderAndResourceId(
            viewType,
            out var viewHolderType,
            out var resourceId
        );

        if (!result)
        {
            throw new FormViewNotFoundException(viewType);
        }

        var view = LayoutInflater.From(parent.Context)!.Inflate(resourceId!.Value, parent, false);

        var viewHolder = Activator.CreateInstance(viewHolderType!, view) as FormViewHolder;

        return viewHolder!;
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var dataResult = TryGetData(position, out var data, out _);

        if (!dataResult)
        {
            throw new FormViewNotFoundException(position);
        }

        if (holder is not FormViewHolder viewHolder)
        {
            return;
        }

        viewHolder.Initialize(data!);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        _formSubscription?.Dispose();
        _itemsSubscription?.Dispose();

        if (_items is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _form = null;
        _items = null;
    }
}
