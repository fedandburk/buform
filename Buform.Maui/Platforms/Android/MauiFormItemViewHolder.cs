using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Fedandburk.Common.Extensions;
using Microsoft.Maui.Platform;
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

        var mauiContext = Application.Current?.Handler?.MauiContext;
        if (mauiContext == null)
        {
            BindFallback(context, item);
            return;
        }

        _container.RemoveAllViews();

        _formItemView = (Activator.CreateInstance(viewType) as FormItemView)!;
        _formItemView.BindingContext = item;

        _platformView = _formItemView.ToPlatform(mauiContext);

        _container.AddView(
            _platformView,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            )
        );
    }

    private void BindFallback(Context context, object item)
    {
        var title = ResolveItemTitle(item);

        var root = new LinearLayout(context) { Orientation = Orientation.Vertical };

        root.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        var horizontal = (int)(16 * context.Resources!.DisplayMetrics!.Density);
        var vertical = (int)(14 * context.Resources.DisplayMetrics.Density);

        root.SetPadding(horizontal, vertical, horizontal, vertical);
        root.Clickable = true;
        root.Focusable = true;

        var typedValue = new Android.Util.TypedValue();
        context.Theme?.ResolveAttribute(
            Android.Resource.Attribute.SelectableItemBackground,
            typedValue,
            true
        );
        root.SetBackgroundResource(typedValue.ResourceId);

        var titleView = new TextView(context);
        titleView.Text = title;
        titleView.SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
        titleView.SetSingleLine(true);
        titleView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
        titleView.SetTextColor(Android.Graphics.Color.Black);

        root.AddView(titleView);

        if (item is ButtonFormItem buttonItem)
        {
            root.Click += (_, _) =>
            {
                buttonItem.Value.SafeExecute();
            };
        }

        _container.RemoveAllViews();
        _container.AddView(root);
    }

    private static string ResolveItemTitle(object item)
    {
        var itemType = item.GetType();

        return itemType.GetProperty("Label")?.GetValue(item) as string ?? itemType.Name;
    }
}
