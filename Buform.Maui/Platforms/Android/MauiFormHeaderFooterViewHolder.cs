using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Buform;

internal sealed class MauiFormHeaderFooterViewHolder : RecyclerView.ViewHolder
{
    public const string HeaderLabel = "HeaderLabel";
    public const string FooterLabel = "FooterLabel";

    private readonly FrameLayout _container;
    private FormHeaderFooterView? _formHeaderFooterView;
    private AView? _platformView;

    public MauiFormHeaderFooterViewHolder(FrameLayout container)
        : base(container)
    {
        _container = container;
    }

    public void Bind(Context context, object item, FormAdapterItemKind kind)
    {
        var itemType = item.GetType();

        if (!MauiFormPlatform.TryGetCellViewType(itemType, out var viewType) || viewType == null)
        {
            BindFallback(context, item, kind);
            return;
        }

        var mauiContext = Application.Current?.Handler?.MauiContext;
        if (mauiContext == null)
        {
            BindFallback(context, item, kind);
            return;
        }

        _container.RemoveAllViews();

        _formHeaderFooterView = (Activator.CreateInstance(viewType) as FormHeaderFooterView)!;
        _formHeaderFooterView.BindingContext = item;

        _platformView = _formHeaderFooterView.ToPlatform(mauiContext);

        _container.AddView(
            _platformView,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            )
        );
    }

    private void BindFallback(Context context, object item, FormAdapterItemKind kind)
    {
        var title = ResolveItemTitle(item, kind);

        var density = context.Resources?.DisplayMetrics?.Density ?? 1f;
        int Dp(int value) => (int)(value * density);

        var root = new LinearLayout(context) { Orientation = Orientation.Vertical };

        root.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        root.Clickable = false;
        root.Focusable = false;

        var titleView = new TextView(context);
        titleView.Text = title;

        switch (kind)
        {
            case FormAdapterItemKind.SectionHeader:
                root.SetPadding(Dp(16), Dp(24), Dp(16), Dp(8));

                titleView.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
                titleView.SetSingleLine(false);
                titleView.SetTextColor(Android.Graphics.Color.Rgb(90, 90, 90));
                titleView.Typeface = Android.Graphics.Typeface.Create(
                    "sans-serif-medium",
                    Android.Graphics.TypefaceStyle.Normal
                );
                break;

            case FormAdapterItemKind.SectionFooter:
                root.SetPadding(Dp(16), Dp(4), Dp(16), Dp(16));

                titleView.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
                titleView.SetSingleLine(false);
                titleView.SetTextColor(Android.Graphics.Color.Rgb(120, 120, 120));
                titleView.Typeface = Android.Graphics.Typeface.Create(
                    "sans-serif",
                    Android.Graphics.TypefaceStyle.Normal
                );
                titleView.SetLineSpacing(0, 1.1f);
                break;

            default:
                root.SetPadding(Dp(16), Dp(8), Dp(16), Dp(8));

                titleView.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
                titleView.SetSingleLine(false);
                titleView.SetTextColor(Android.Graphics.Color.Rgb(90, 90, 90));
                break;
        }

        root.AddView(titleView);

        _container.RemoveAllViews();
        _container.AddView(root);
    }

    private static string ResolveItemTitle(object item, FormAdapterItemKind kind)
    {
        var itemType = item.GetType();

        return kind switch
        {
            FormAdapterItemKind.SectionHeader
                => itemType.GetProperty(HeaderLabel)?.GetValue(item) as string ?? itemType.Name,

            FormAdapterItemKind.SectionFooter
                => itemType.GetProperty(FooterLabel)?.GetValue(item) as string ?? itemType.Name,

            _ => itemType.Name,
        };
    }
}
