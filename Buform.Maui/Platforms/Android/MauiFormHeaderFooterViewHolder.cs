using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Buform;

internal sealed class MauiFormHeaderFooterViewHolder : RecyclerView.ViewHolder
{
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
        Type? viewType = kind switch
        {
            FormAdapterItemKind.SectionHeader => ResolveHeaderViewType(item.GetType()),
            FormAdapterItemKind.SectionFooter => ResolveFooterViewType(item.GetType()),
            _ => null,
        };

        if (viewType == null)
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

        MeasureAndArrange();
    }

    public void Unbind()
    {
        _formHeaderFooterView = null;
        _platformView = null;
        _container.RemoveAllViews();
    }

    private static Type? ResolveHeaderViewType(Type itemType)
    {
        return MauiFormPlatform.TryGetHeaderViewType(itemType, out var viewType) ? viewType : null;
    }

    private static Type? ResolveFooterViewType(Type itemType)
    {
        return MauiFormPlatform.TryGetFooterViewType(itemType, out var viewType) ? viewType : null;
    }

    private void MeasureAndArrange()
    {
        if (_formHeaderFooterView == null || _platformView == null || _container.Width <= 0)
        {
            return;
        }

        var width = _container.Width - _container.PaddingLeft - _container.PaddingRight;

        if (width <= 0)
        {
            return;
        }

        var measured = _formHeaderFooterView.Measure(width, double.PositiveInfinity);
        var height = (int)Math.Ceiling(measured.Height);

        _formHeaderFooterView.Arrange(new Rect(0, 0, width, height));

        _platformView.LayoutParameters = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            height
        );
    }

    private void BindFallback(Context context, object item, FormAdapterItemKind kind)
    {
        var text = kind switch
        {
            FormAdapterItemKind.SectionHeader
                => FormReflectionHelper.GetHeaderLabel(item) ?? item.GetType().Name,

            FormAdapterItemKind.SectionFooter
                => FormReflectionHelper.GetFooterLabel(item) ?? item.GetType().Name,

            _ => item.GetType().Name,
        };

        var density = context.Resources?.DisplayMetrics?.Density ?? 1f;
        int Dp(int value) => (int)(value * density);

        var textView = new TextView(context);
        textView.Text = text;
        textView.SetSingleLine(false);

        var root = new LinearLayout(context) { Orientation = Orientation.Vertical };
        root.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        switch (kind)
        {
            case FormAdapterItemKind.SectionHeader:
                root.SetPadding(Dp(16), Dp(24), Dp(16), Dp(8));
                textView.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
                break;

            case FormAdapterItemKind.SectionFooter:
                root.SetPadding(Dp(16), Dp(4), Dp(16), Dp(16));
                textView.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
                break;
        }

        root.AddView(textView);

        _container.RemoveAllViews();
        _container.AddView(root);
    }
}
