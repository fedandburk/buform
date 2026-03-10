using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

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

    public override int GetItemViewType(int position)
        => (int)_items[position].Kind;

    public void SetForm(Form? form)
    {
        _form = form;
        RebuildItems();
        NotifyDataSetChanged();
    }

    private void RebuildItems()
    {
        _items.Clear();

        if (_form is null)
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
        var container = CreateContainer();

        return (FormAdapterItemKind)viewType switch
        {
            FormAdapterItemKind.Row => new MauiFormItemViewHolder(container),
            FormAdapterItemKind.SectionHeader => new MauiFormHeaderFooterViewHolder(container),
            FormAdapterItemKind.SectionFooter => new MauiFormHeaderFooterViewHolder(container),
            _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null)
        };
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var adapterItem = _items[position];
        var item = adapterItem.Item;

        if (item == null)
        {
            return;
        }
        
        switch (adapterItem.Kind)
        {
            case FormAdapterItemKind.SectionHeader:
                if (holder is MauiFormHeaderFooterViewHolder headerHolder)
                {
                    headerHolder.Bind(_context, item,FormAdapterItemKind.SectionHeader);
                }
                break;
            
            case FormAdapterItemKind.Row:
                if (holder is MauiFormItemViewHolder rowHolder)
                {
                    rowHolder.Bind(_context, item);
                }
                break;

            case FormAdapterItemKind.SectionFooter:
                if (holder is MauiFormHeaderFooterViewHolder footerHolder)
                {
                    footerHolder.Bind(_context, item, FormAdapterItemKind.SectionFooter);
                }
                break;
        }
    }

    private FrameLayout CreateContainer()
    {
        var container = new FrameLayout(_context);
        container.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent);

        return container;
    }

    private static bool HasHeader(object section)
    {
        var title = section.GetType().GetProperty("HeaderLabel")?.GetValue(section) as string;
        return !string.IsNullOrWhiteSpace(title);
    }

    private static bool HasFooter(object section)
    {
        var footer = section.GetType().GetProperty("FooterLabel")?.GetValue(section) as string;

        return !string.IsNullOrWhiteSpace(footer);
    }
}

