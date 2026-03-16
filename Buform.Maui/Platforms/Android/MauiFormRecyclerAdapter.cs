using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Object = Java.Lang.Object;

namespace Buform;

internal sealed class MauiFormRecyclerAdapter : RecyclerView.Adapter
{
    private readonly Context _context;
    private readonly IMauiContext _mauiContext;
    private readonly List<FormAdapterItem> _items = [];

    private Form? _form;

    public MauiFormRecyclerAdapter(Context context, IMauiContext mauiContext)
    {
        _context = context;
        _mauiContext = mauiContext;
    }

    public override int ItemCount => _items.Count;

    public override int GetItemViewType(int position) => (int)_items[position].HolderType;

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

            default:
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

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var container = CreateContainer(parent.Context);

        return (FormViewHolderType)viewType switch
        {
            FormViewHolderType.Item => new MauiFormItemViewHolder(container, _mauiContext),

            FormViewHolderType.Header
                => new MauiFormHeaderFooterViewHolder(container, _mauiContext),

            FormViewHolderType.Footer
                => new MauiFormHeaderFooterViewHolder(container, _mauiContext),

            _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null),
        };
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var adapterItem = _items[position];

        switch (adapterItem.HolderType)
        {
            case FormViewHolderType.Item:
                ((MauiFormItemViewHolder)holder).Unbind();
                ((MauiFormItemViewHolder)holder).Bind(_context, adapterItem.Item);
                break;

            case FormViewHolderType.Header:
                ((MauiFormHeaderFooterViewHolder)holder).Unbind();
                ((MauiFormHeaderFooterViewHolder)holder).Bind(
                    _context,
                    adapterItem.Item,
                    FormViewHolderType.Header
                );
                break;

            case FormViewHolderType.Footer:
                ((MauiFormHeaderFooterViewHolder)holder).Unbind();
                ((MauiFormHeaderFooterViewHolder)holder).Bind(
                    _context,
                    adapterItem.Item,
                    FormViewHolderType.Footer
                );
                break;

            default:
                break;
        }
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

    private static FrameLayout CreateContainer(Context context)
    {
        var container = new FrameLayout(context);
        container.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        return container;
    }

    private static bool HasHeader(object section)
    {
        var sectionType = section.GetType();

        return !string.IsNullOrWhiteSpace(FormReflectionHelper.GetHeaderLabel(section))
            || MauiFormPlatform.TryGetHeaderViewType(sectionType, out _);
    }

    private static bool HasFooter(object section)
    {
        var sectionType = section.GetType();

        return !string.IsNullOrWhiteSpace(FormReflectionHelper.GetFooterLabel(section))
            || MauiFormPlatform.TryGetFooterViewType(sectionType, out _);
    }
}
