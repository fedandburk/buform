using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AView = Android.Views.View;

namespace Buform;

internal sealed class SectionHeaderViewHolder : RecyclerView.ViewHolder
{
    private readonly TextView _textView;

    public SectionHeaderViewHolder(AView itemView) : base(itemView)
    {
        _textView = (TextView)itemView;
    }

    public void Bind(FormAdapterItem item)
    {
        _textView.Text = item.Title ?? string.Empty;
    }
}

