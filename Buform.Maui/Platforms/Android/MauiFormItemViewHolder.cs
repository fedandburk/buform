using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Buform;

internal sealed class MauiFormItemViewHolder : MauiFormViewHolderBase<FormItemView>
{
    private LinearLayout? _fallbackRoot;
    private TextView? _fallbackTitleView;

    private FormViewHolder? _legacyViewHolder;

    public MauiFormItemViewHolder(FrameLayout container, IMauiContext mauiContext)
        : base(container, mauiContext) { }

    public void Bind(Context context, object item)
    {
        var itemType = item.GetType();

        if (
            TryBindMauiView(
                item,
                MauiFormPlatform.TryGetCellViewType(itemType, out var mauiViewType)
                    ? mauiViewType
                    : null
            )
        )
        {
            return;
        }

        if (TryBindLegacyView(context, item))
        {
            return;
        }

        BindFallback(context, item);
    }

    public override void Unbind()
    {
        UnbindLegacy();
        UnbindFallback();

        _container.RemoveAllViews();
    }

    private bool TryBindLegacyView(Context context, object item)
    {
        var itemType = item.GetType();

        if (!FormPlatform.TryGetItemViewType(itemType, out var viewType) || viewType == null)
        {
            return false;
        }

        var result = FormPlatform.TryGetViewHolderAndResourceId(
            viewType.Value,
            out var viewHolderType,
            out var resourceId
        );

        if (!result || viewHolderType == null || resourceId == null)
        {
            return false;
        }

        var view = LayoutInflater.From(context)?.Inflate(resourceId.Value, _container, false);
        if (view == null)
        {
            return false;
        }

        if (Activator.CreateInstance(viewHolderType, view) is not FormViewHolder viewHolder)
        {
            return false;
        }

        _container.RemoveAllViews();

        _legacyViewHolder = viewHolder;

        _container.AddView(view);

        if (item is not IDisposable disposable)
        {
            return false;
        }

        viewHolder.Initialize(disposable);

        return true;
    }

    private void BindFallback(Context context, object item)
    {
        EnsureFallbackView(context);

        if (_fallbackRoot == null || _fallbackTitleView == null)
        {
            return;
        }

        _container.RemoveAllViews();
        _container.AddView(_fallbackRoot);

        _fallbackTitleView.Text = item.GetType().Name;
        _fallbackTitleView.Enabled = false;
    }

    private void EnsureFallbackView(Context context)
    {
        if (_fallbackRoot != null && _fallbackTitleView != null)
        {
            return;
        }

        _fallbackRoot = new LinearLayout(context) { Orientation = Orientation.Vertical };

        _fallbackTitleView = new TextView(context);
        _fallbackTitleView.SetSingleLine(true);
        _fallbackTitleView.Ellipsize = TextUtils.TruncateAt.End;

        _fallbackRoot.AddView(_fallbackTitleView);
    }

    private void UnbindLegacy()
    {
        if (_legacyViewHolder is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _legacyViewHolder = null;
    }

    private void UnbindFallback()
    {
        if (_fallbackTitleView == null)
            return;

        _fallbackTitleView.Text = null;
        _fallbackTitleView.Enabled = true;
    }
}
