using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AView = Android.Views.View;

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

        foreach (var group in _form)
        {
            foreach (var row in group)
            {
                _items.Add(FormAdapterItem.CreateRow(row));
            }
        }
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        return (FormAdapterItemKind)viewType switch
        {
            FormAdapterItemKind.Row => new FormRowViewHolder(CreateRowView()),
            _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null)
        };
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var item = _items[position];

        switch (holder)
        {
            case FormRowViewHolder row:
                row.Bind(item);
                break;
        }
    }

    private AView CreateRowView()
    {
        var container = new LinearLayout(_context)
        {
            Orientation = Orientation.Vertical
        };

        container.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent);

        var textView = new TextView(_context);
        textView.LayoutParameters = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent);

        textView.SetPadding(DpToPx(16), DpToPx(16), DpToPx(16), DpToPx(16));
        textView.TextSize = 16;

        container.AddView(textView);

        return container;
    }

    private int DpToPx(int dp)
    {
        return (int)TypedValue.ApplyDimension(
            ComplexUnitType.Dip,
            dp,
            _context.Resources?.DisplayMetrics);
    }
}

