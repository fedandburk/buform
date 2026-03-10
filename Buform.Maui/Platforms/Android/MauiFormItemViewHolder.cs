using Android.Content;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Platform;
using AndroidX.RecyclerView.Widget;
using AView = Android.Views.View;

namespace Buform;

internal sealed class MauiFormItemViewHolder : RecyclerView.ViewHolder
{
    private readonly FrameLayout _container;
    private FormItemView? _formItemView;
    private AView? _platformView;

    public MauiFormItemViewHolder(FrameLayout container)
        : base(container)
    {
        _container = container;
    }

    public void Bind(Context context, object item)
    {
        var itemType = item.GetType();

        if (!MauiFormPlatform.TryGetCellViewType(itemType, out var viewType) || viewType == null)
        {
            BindFallback(context, item);
            return;
        }

        _container.RemoveAllViews();

        _formItemView = (Activator.CreateInstance(viewType) as FormItemView)!;
        _formItemView.BindingContext = item;

        var mauiContext = Application.Current?.Handler?.MauiContext;
        if (mauiContext == null)
        {
            return;
        }

        _platformView = _formItemView.ToPlatform(mauiContext);

        _container.AddView(
            _platformView,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent));
    }

    private void BindFallback(Context context, object item)
    {
        var textView = new TextView(context);
        textView.Text = ResolveItemTitle(item);
        textView.SetPadding(32, 32, 32, 32);

        _container.RemoveAllViews();
        _container.AddView(textView);
    }

    private static string ResolveItemTitle(object item)
    {
        var itemType = item.GetType();

        return itemType.GetProperty("Label")?.GetValue(item) as string
               ?? itemType.Name;
    }
}