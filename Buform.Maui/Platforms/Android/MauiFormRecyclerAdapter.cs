using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Object = Java.Lang.Object;

namespace Buform;

internal sealed class MauiFormRecyclerAdapter : RecyclerView.Adapter
{
    private readonly Context _context;
    private readonly List<FormAdapterItem> _items = [];
    private Form? _form;

    public MauiFormRecyclerAdapter(Context context)
    {
        _context = context;
    }

    public override int ItemCount => _items.Count;

    public override int GetItemViewType(int position) => (int)_items[position].Kind;

    public override long GetItemId(int position) => position;

    public override void OnViewRecycled(Object holder)
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

        base.OnViewRecycled(holder);
    }

    public void SetForm(Form? form)
    {
        _form = form;
        RebuildItems();
        NotifyDataSetChanged();
    }

    private void RebuildItems()
    {
        _items.Clear();

        if (_form == null)
        {
            return;
        }

        foreach (var section in _form)
        {
            if (HasHeader(section))
            {
                _items.Add(FormAdapterItem.CreateSectionHeader(section));
            }

            foreach (var row in section)
            {
                _items.Add(FormAdapterItem.CreateRow(row));
            }

            if (HasFooter(section))
            {
                _items.Add(FormAdapterItem.CreateSectionFooter(section));
            }
        }
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var container = CreateContainer(parent.Context);

        return (FormAdapterItemKind)viewType switch
        {
            FormAdapterItemKind.Row => new MauiFormItemViewHolder(container),
            FormAdapterItemKind.SectionHeader => new MauiFormHeaderFooterViewHolder(container),
            FormAdapterItemKind.SectionFooter => new MauiFormHeaderFooterViewHolder(container),
            _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null),
        };
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var adapterItem = _items[position];

        switch (adapterItem.Kind)
        {
            case FormAdapterItemKind.Row:
                ((MauiFormItemViewHolder)holder).Bind(_context, adapterItem.Item);
                break;

            case FormAdapterItemKind.SectionHeader:
                ((MauiFormHeaderFooterViewHolder)holder).Bind(
                    _context,
                    adapterItem.Item,
                    FormAdapterItemKind.SectionHeader
                );
                break;

            case FormAdapterItemKind.SectionFooter:
                ((MauiFormHeaderFooterViewHolder)holder).Bind(
                    _context,
                    adapterItem.Item,
                    FormAdapterItemKind.SectionFooter
                );
                break;
        }
    }

    private static FrameLayout CreateContainer(Context context)
    {
        var container = new FrameLayout(context);
        container.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        return container;
    }

    private static bool HasHeader(object section) =>
        !string.IsNullOrWhiteSpace(FormReflectionHelper.GetHeaderLabel(section));

    private static bool HasFooter(object section) =>
        !string.IsNullOrWhiteSpace(FormReflectionHelper.GetFooterLabel(section));
}
