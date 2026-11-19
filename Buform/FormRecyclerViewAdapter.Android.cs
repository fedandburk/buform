using System.Collections.Specialized;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

internal sealed class FormRecyclerViewAdapter : RecyclerView.Adapter, IFormRowSource
{
    private readonly FormGroupHandlerList _groupHandlers;

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

            _groupHandlers.Clear();

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

    public FormRecyclerViewAdapter()
    {
        _groupHandlers = new FormGroupHandlerList();
    }

    private void OnRowsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        // TODO: Use Looper.MainLooper.
        FormRowAppearance.NotifyRowsChanged(this, args);
    }

    public FormRow GetRow(int position)
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
        var dataType = row.Data.GetType();

        int? viewType;

        var result = row.Type switch
        {
            FormRowType.Item => FormPlatform.TryGetItemViewType(dataType, out viewType),
            FormRowType.Header => FormPlatform.TryGetGroupHeaderViewType(dataType, out viewType),
            FormRowType.Footer => FormPlatform.TryGetGroupFooterViewType(dataType, out viewType),
            _ => throw new ArgumentOutOfRangeException(nameof(position), row.Type, null),
        };

        if (!result)
        {
            throw new FormViewNotFoundException(dataType);
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

        var view = LayoutInflater.From(parent.Context)!.Inflate(resourceId!.Value, parent, false)!;

        return FormPlatform.CreateViewHolder(viewHolderType!, view);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var row = GetRow(position);

        if (holder is not FormViewHolder viewHolder)
        {
            throw new ArgumentException(
                $"The view holder must be of type '{typeof(FormViewHolder).FullName}'.",
                nameof(holder)
            );
        }

        viewHolder.Initialize(row.Data);

        if (row.Item != null)
        {
            var groupHandler = _groupHandlers.Get(row.Group);

            FormItemClickListener.Attach(viewHolder.ItemView, groupHandler, row.Item);
        }

        FormRowAppearance.Apply(viewHolder.ItemView, row);
    }

    public override void OnBindViewHolder(
        RecyclerView.ViewHolder holder,
        int position,
        IList<Java.Lang.Object> payloads
    )
    {
        if (!FormRowAppearance.IsAppearanceUpdate(payloads))
        {
            OnBindViewHolder(holder, position);

            return;
        }

        FormRowAppearance.Apply(holder.ItemView, GetRow(position));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeRows();

            _groupHandlers.Clear();

            _form = null;
        }

        base.Dispose(disposing);
    }
}
