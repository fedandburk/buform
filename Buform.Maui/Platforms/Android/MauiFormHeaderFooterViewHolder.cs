using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Buform;

internal sealed class MauiFormHeaderFooterViewHolder : MauiFormViewHolderBase<FormHeaderFooterView>
{
    private LinearLayout? _fallbackRoot;
    private TextView? _fallbackTextView;

    public MauiFormHeaderFooterViewHolder(FrameLayout container, IMauiContext mauiContext)
        : base(container, mauiContext) { }

    public void Bind(Context context, object item, FormAdapterItemKind kind)
    {
        if (TryBindMauiView(item, ResolveViewType(item.GetType(), kind)))
        {
            return;
        }

        BindFallback(context, item, kind);
    }

    private static Type? ResolveViewType(Type itemType, FormAdapterItemKind kind)
    {
        return kind switch
        {
            FormAdapterItemKind.SectionHeader
                => MauiFormPlatform.TryGetHeaderViewType(itemType, out var headerViewType)
                    ? headerViewType
                    : null,

            FormAdapterItemKind.SectionFooter
                => MauiFormPlatform.TryGetFooterViewType(itemType, out var footerViewType)
                    ? footerViewType
                    : null,

            _ => null,
        };
    }

    private void BindFallback(Context context, object item, FormAdapterItemKind kind)
    {
        EnsureFallbackView(context);

        if (_fallbackRoot == null || _fallbackTextView == null)
        {
            return;
        }

        _fallbackTextView.Text = kind switch
        {
            FormAdapterItemKind.SectionHeader
                => FormReflectionHelper.GetHeaderLabel(item) ?? item.GetType().Name,

            FormAdapterItemKind.SectionFooter
                => FormReflectionHelper.GetFooterLabel(item) ?? item.GetType().Name,

            _ => item.GetType().Name,
        };

        var density = context.Resources?.DisplayMetrics?.Density ?? 1f;
        int Dp(int value) => (int)(value * density);

        switch (kind)
        {
            case FormAdapterItemKind.SectionHeader:
                _fallbackRoot.SetPadding(Dp(16), Dp(24), Dp(16), Dp(8));
                _fallbackTextView.SetTextSize(ComplexUnitType.Sp, 14);
                _fallbackTextView.SetAllCaps(false);
                break;
            case FormAdapterItemKind.SectionFooter:
                _fallbackRoot.SetPadding(Dp(16), Dp(4), Dp(16), Dp(16));
                _fallbackTextView.SetTextSize(ComplexUnitType.Sp, 12);
                _fallbackTextView.SetAllCaps(false);
                break;
            case FormAdapterItemKind.Row:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }

        ShowFallback(_fallbackRoot);
    }

    private void EnsureFallbackView(Context context)
    {
        if (_fallbackRoot != null)
        {
            return;
        }

        var root = new LinearLayout(context) { Orientation = Orientation.Vertical };
        root.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        var textView = new TextView(context);
        textView.SetSingleLine(false);

        root.AddView(textView);

        _fallbackRoot = root;
        _fallbackTextView = textView;
    }
}
