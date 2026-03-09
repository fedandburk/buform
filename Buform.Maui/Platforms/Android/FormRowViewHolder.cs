using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AView = Android.Views.View;

namespace Buform;

internal sealed class FormRowViewHolder : RecyclerView.ViewHolder
{
    private readonly TextView _titleView;

    public FormRowViewHolder(AView itemView) : base(itemView)
    {
        var container = (LinearLayout)itemView;
        _titleView = (TextView)container.GetChildAt(0)!;
    }

    public void Bind(FormAdapterItem item)
    {
        if (item.Item is null)
        {
            _titleView.Text = string.Empty;
            return;
        }

        _titleView.Text = ResolveItemTitle(item.Item);
    }

    private static string ResolveItemTitle(object item)
    {
        var itemType = item.GetType();

        var label =
            itemType.GetProperty("Label")?.GetValue(item) as string
            ?? itemType.GetProperty("Title")?.GetValue(item) as string
            ?? itemType.Name;

        return label;
    }
}