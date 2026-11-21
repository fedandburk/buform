using System.Collections.Specialized;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Object = Java.Lang.Object;

namespace Buform;

internal sealed class MauiFormRecyclerAdapter : RecyclerView.Adapter, IFormRowSource
{
    private readonly IMauiContext _mauiContext;
    private readonly FormGroupHandlerList _groupHandlers;

    private Form? _form;
    private FormRowList? _rows;

    public MauiFormRecyclerAdapter(IMauiContext mauiContext)
    {
        ArgumentNullException.ThrowIfNull(mauiContext);

        _mauiContext = mauiContext;

        _groupHandlers = new FormGroupHandlerList();
    }

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

            NotifyDataSetChanged();
        }
    }

    public override int ItemCount => _rows?.Count ?? 0;

    public override int GetItemViewType(int position)
    {
        return (int)GetRow(position).Type;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var context = parent.Context
            ?? throw new InvalidOperationException("Parent view has no context.");

        var container = new FrameLayout(context)
        {
            LayoutParameters = new RecyclerView.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            ),
        };

        return (FormRowType)viewType switch
        {
            FormRowType.Item => new MauiFormItemViewHolder(container, _mauiContext),
            FormRowType.Header or FormRowType.Footer => new MauiFormHeaderFooterViewHolder(
                container,
                _mauiContext
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null),
        };
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var row = GetRow(position);

        switch (holder)
        {
            case MauiFormItemViewHolder itemHolder:
                BindItem(itemHolder, row);
                break;
            case MauiFormHeaderFooterViewHolder headerFooterHolder:
                headerFooterHolder.Unbind();
                headerFooterHolder.Bind(row.Group, row.Type);
                break;
            default:
                throw new FormViewNotFoundException(position);
        }
    }

    public override void OnBindViewHolder(
        RecyclerView.ViewHolder holder,
        int position,
        IList<Object> payloads
    )
    {
        if (!FormRowAppearance.IsAppearanceUpdate(payloads))
        {
            OnBindViewHolder(holder, position);

            return;
        }

        if (holder is not MauiFormItemViewHolder itemHolder)
        {
            return;
        }

        ApplyAppearance(itemHolder, GetRow(position));
    }

    public override void OnViewRecycled(Object holder)
    {
        Unbind(holder);

        base.OnViewRecycled(holder);
    }

    public override bool OnFailedToRecycleView(Object holder)
    {
        Unbind(holder);

        return base.OnFailedToRecycleView(holder);
    }

    public FormRow GetRow(int position)
    {
        if (_rows == null || position < 0 || position >= _rows.Count)
        {
            throw new FormViewNotFoundException(position);
        }

        return _rows[position];
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

    private static void ApplyAppearance(MauiFormItemViewHolder holder, FormRow row)
    {
        var contentView = holder.ContentView;

        if (contentView == null)
        {
            return;
        }

        FormRowAppearance.Apply(contentView, row);
    }

    private static void Unbind(Object holder)
    {
        switch (holder)
        {
            case MauiFormItemViewHolder itemHolder:
                itemHolder.Unbind();
                break;
            case MauiFormHeaderFooterViewHolder headerFooterHolder:
                headerFooterHolder.Unbind();
                break;
        }
    }

    private void BindItem(MauiFormItemViewHolder holder, FormRow row)
    {
        var item = row.Item ?? throw new InvalidOperationException("The item row has no item.");

        holder.Unbind();
        holder.Bind(item);

        var groupHandler = _groupHandlers.Get(row.Group);

        FormItemClickListener.Attach(holder.ItemView, groupHandler, item);

        ApplyAppearance(holder, row);
    }

    private void OnRowsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        FormRowAppearance.NotifyRowsChanged(this, args);
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
}
