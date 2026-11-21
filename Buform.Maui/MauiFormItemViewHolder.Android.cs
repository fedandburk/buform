using Android.Views;
using Android.Widget;
using Google.Android.Material.ListItem;
using AView = Android.Views.View;

namespace Buform;

internal sealed class MauiFormItemViewHolder : MauiFormViewHolder<FormItemView>
{
    private ListItemLayout? _cardLayout;
    private ListItemCardView? _card;

    public MauiFormItemViewHolder(FrameLayout container, IMauiContext mauiContext)
        : base(container, mauiContext)
    {
        /* Required constructor */
    }

    public void Bind(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var itemType = item.GetType();

        if (MauiFormPlatform.TryGetCellViewType(itemType, out var mauiViewType))
        {
            BindMauiView(item, mauiViewType!);

            return;
        }

        BindNativeView(item);
    }

    private void BindNativeView(IFormItem item)
    {
        var itemType = item.GetType();

        if (!FormPlatform.TryGetItemViewType(itemType, out var viewType))
        {
            throw new FormViewNotFoundException(itemType);
        }

        BindNativeView(viewType!.Value, item);
    }

    protected override AView WrapPlatformView(AView platformView)
    {
        ArgumentNullException.ThrowIfNull(platformView);

        if (_cardLayout == null || _card == null)
        {
            _cardLayout = FormRowAppearance.InflateCard(Container, out var card);
            _card = card;
        }

        if (Equals(platformView.Parent, _card))
        {
            return _cardLayout;
        }

        (platformView.Parent as ViewGroup)?.RemoveView(platformView);

        _card.RemoveAllViews();
        _card.AddView(platformView);

        return _cardLayout;
    }

    protected override int GetHorizontalInset()
    {
        if (_cardLayout == null || _card == null)
        {
            return 0;
        }

        return _cardLayout.PaddingLeft
            + _cardLayout.PaddingRight
            + _card.ContentPaddingLeft
            + _card.ContentPaddingRight;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _card = null;
            _cardLayout = null;
        }

        base.Dispose(disposing);
    }
}
