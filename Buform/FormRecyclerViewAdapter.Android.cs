using System.Collections.Specialized;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

internal sealed class FormRecyclerViewAdapter : RecyclerView.Adapter
{
    private Form? _form;
    private FormRowList? _rows;

    public Form? Form
    {
        get => _form;
        set
        {
            if (ReferenceEquals(_form, value))
            {
                return;
            }

            DisposeRows();

            _form = value;

            if (_form != null)
            {
                _rows = new FormRowList(_form);
                _rows.CollectionChanged += OnRowsChanged;
            }

            // TODO: Use Looper.MainLooper.
            NotifyDataSetChanged();
        }
    }

    public override int ItemCount => _rows?.Count ?? 0;

    private void OnRowsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        // TODO: Use Looper.MainLooper.
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                NotifyItemRangeInserted(args.NewStartingIndex, args.NewItems!.Count);
                break;
            case NotifyCollectionChangedAction.Remove:
                NotifyItemRangeRemoved(args.OldStartingIndex, args.OldItems!.Count);
                break;
            case NotifyCollectionChangedAction.Replace:
                NotifyItemRangeChanged(args.NewStartingIndex, args.NewItems!.Count);
                break;
            case NotifyCollectionChangedAction.Move:
                for (var i = 0; i < args.NewItems!.Count; i++)
                {
                    NotifyItemMoved(args.OldStartingIndex + i, args.NewStartingIndex + i);
                }

                break;
            case NotifyCollectionChangedAction.Reset:
                NotifyDataSetChanged();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args), args.Action, null);
        }
    }

    private FormRow GetRow(int position)
    {
        if (_rows == null || position < 0 || position >= _rows.Count)
        {
            throw new FormViewNotFoundException(position);
        }

        return _rows[position];
    }

    private void DisposeRows()
    {
        if (_rows == null)
        {
            return;
        }

        _rows.CollectionChanged -= OnRowsChanged;
        _rows.Dispose();
        _rows = null;
    }

    public override int GetItemViewType(int position)
    {
        var row = GetRow(position);
        var data = row.Data;
        var dataType = data.GetType();

        int? viewType;

        var result = row.Type switch
        {
            FormRowType.Item => FormPlatform.TryGetItemViewType(dataType, out viewType),
            FormRowType.Header => FormPlatform.TryGetGroupHeaderViewType(dataType, out viewType),
            FormRowType.Footer => FormPlatform.TryGetGroupFooterViewType(dataType, out viewType),
            _ => throw new ArgumentOutOfRangeException(),
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
        var row = GetRow(position);

        if (holder is not FormViewHolder viewHolder)
        {
            return;
        }

        viewHolder.Initialize(row.Data);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeRows();

            _form = null;
        }

        base.Dispose(disposing);
    }
}
