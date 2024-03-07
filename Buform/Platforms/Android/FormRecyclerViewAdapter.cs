using System.Collections.Specialized;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Fedandburk.Common.Extensions;

namespace Buform;

public class FormRecyclerViewAdapter : RecyclerView.Adapter
{
    private Form? _form;
    private IEnumerable<IFormItem>? _items;
    private IDisposable? _subscription;

    public Form? Form
    {
        get => _form;
        set
        {
            if (ReferenceEquals(Form, value))
            {
                return;
            }

            _form = value;

            if (_items is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _subscription?.Dispose();

            if (Form == null)
            {
                return;
            }

            _items = Form.ObservableFlatten();

            if (_items is INotifyCollectionChanged notifyCollectionChanged)
            {
                _subscription = notifyCollectionChanged.WeakSubscribe(OnItemsChanged);
            }

            // TODO: Use Looper.MainLooper.
            NotifyDataSetChanged();
        }
    }

    public override int ItemCount => _items?.Count() ?? 0;

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (_items == null || _subscription == null)
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

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is not FormViewHolder viewHolder)
        {
            return;
        }

        var item = _items?.ElementAtOrDefault(position);

        if (item == null)
        {
            return;
        }

        viewHolder.Initialize(item);
    }

    public override int GetItemViewType(int position)
    {
        var item = _items?.ElementAtOrDefault(position);

        if (item == null)
        {
            return int.MinValue;
        }

        if (!FormPlatform.TryGetViewType(item.GetType(), out var viewType))
        {
            throw new FormViewNotFoundException(item);
        }

        return viewType!.Value;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        if (
            !FormPlatform.TryGetViewHolderAndResourceId(
                viewType,
                out var viewHolderType,
                out var resourceId
            )
        )
        {
            throw new FormViewNotFoundException(viewType);
        }

        var view = LayoutInflater.From(parent.Context)!.Inflate(resourceId!.Value, parent, false);

        var viewHolder = Activator.CreateInstance(viewHolderType!, view) as RecyclerView.ViewHolder;

        return viewHolder!;
    }
}
