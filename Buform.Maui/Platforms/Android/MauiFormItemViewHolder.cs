using Android.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Fedandburk.Common.Extensions;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Buform;

internal sealed class MauiFormItemViewHolder : MauiFormViewHolderBase<FormItemView>
{
    private LinearLayout? _fallbackRoot;
    private TextView? _fallbackTitleView;
    private ButtonFormItem? _fallbackButtonItem;

    public MauiFormItemViewHolder(FrameLayout container, IMauiContext mauiContext)
        : base(container, mauiContext) { }

    public void Bind(Context context, object item)
    {
        var itemType = item.GetType();

        if (
            TryBindMauiView(
                item,
                MauiFormPlatform.TryGetCellViewType(itemType, out var viewType) ? viewType : null
            )
        )
        {
            return;
        }

        BindFallback(context, item);
    }

    private void BindFallback(Context context, object item)
    {
        EnsureFallbackView(context);

        if (_fallbackRoot == null || _fallbackTitleView == null)
        {
            return;
        }

        var title =
            FormReflectionHelper.GetLabel(item)
            ?? FormReflectionHelper.GetFormattedValue(item)
            ?? item.GetType().Name;

        _fallbackTitleView.Text = title;
        _fallbackButtonItem = item as ButtonFormItem;

        ShowFallback(_fallbackRoot);
    }

    private void EnsureFallbackView(Context context)
    {
        if (_fallbackRoot != null)
        {
            return;
        }

        var density = context.Resources?.DisplayMetrics?.Density ?? 1f;
        int Dp(int value) => (int)(value * density);

        var root = new LinearLayout(context) { Orientation = Orientation.Vertical };
        root.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        root.SetPadding(Dp(16), Dp(14), Dp(16), Dp(14));
        root.Clickable = true;
        root.Focusable = true;

        var typedValue = new TypedValue();
        context.Theme?.ResolveAttribute(
            Android.Resource.Attribute.SelectableItemBackground,
            typedValue,
            true
        );
        root.SetBackgroundResource(typedValue.ResourceId);

        var titleView = new TextView(context);
        titleView.SetSingleLine(true);
        titleView.Ellipsize = TextUtils.TruncateAt.End;
        titleView.SetTextSize(ComplexUnitType.Sp, 16);

        if (
            context.Theme?.ResolveAttribute(
                Android.Resource.Attribute.TextColorPrimary,
                typedValue,
                true
            ) == true
        )
        {
            var color = new Android.Graphics.Color(
                ContextCompat.GetColor(context, typedValue.ResourceId)
            );

            titleView.SetTextColor(color);
        }

        root.AddView(titleView);
        root.Click += OnFallbackRootClick;

        _fallbackRoot = root;
        _fallbackTitleView = titleView;
    }

    private void OnFallbackRootClick(object? sender, EventArgs e)
    {
        _fallbackButtonItem?.Value.SafeExecute();
    }
}
