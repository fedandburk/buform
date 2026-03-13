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

    public void Bind(Context context, object item, FormViewHolderType holderType)
    {
        if (TryBindMauiView(item, ResolveViewType(item.GetType(), holderType)))
        {
            return;
        }

        BindFallback(context, item, holderType);
    }

    private static Type? ResolveViewType(Type itemType, FormViewHolderType holderType)
    {
        return holderType switch
        {
            FormViewHolderType.Header
                => MauiFormPlatform.TryGetHeaderViewType(itemType, out var headerViewType)
                    ? headerViewType
                    : null,

            FormViewHolderType.Footer
                => MauiFormPlatform.TryGetFooterViewType(itemType, out var footerViewType)
                    ? footerViewType
                    : null,

            _ => null,
        };
    }

    private void BindFallback(Context context, object item, FormViewHolderType holderType)
    {
        EnsureFallbackView(context);

        if (_fallbackRoot == null || _fallbackTextView == null)
        {
            return;
        }

        _fallbackTextView.Text = holderType switch
        {
            FormViewHolderType.Header
                => FormReflectionHelper.GetHeaderLabel(item) ?? item.GetType().Name,

            FormViewHolderType.Footer
                => FormReflectionHelper.GetFooterLabel(item) ?? item.GetType().Name,

            _ => item.GetType().Name,
        };

        var density = context.Resources?.DisplayMetrics?.Density ?? 1f;
        int Dp(int value) => (int)(value * density);

        switch (holderType)
        {
            case FormViewHolderType.Header:
                _fallbackRoot.SetPadding(Dp(16), Dp(24), Dp(16), Dp(8));
                _fallbackTextView.SetTextSize(ComplexUnitType.Sp, 14);
                _fallbackTextView.SetAllCaps(false);
                break;
            case FormViewHolderType.Footer:
                _fallbackRoot.SetPadding(Dp(16), Dp(4), Dp(16), Dp(16));
                _fallbackTextView.SetTextSize(ComplexUnitType.Sp, 12);
                _fallbackTextView.SetAllCaps(false);
                break;
            case FormViewHolderType.Item:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(holderType), holderType, null);
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
