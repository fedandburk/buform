using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using MView = Microsoft.Maui.Controls.View;

namespace Buform;

internal abstract class MauiFormViewHolderBase<TMauiView> : RecyclerView.ViewHolder
    where TMauiView : MView
{
    private const int MaxLayoutRetries = 3;

    public readonly FrameLayout _container;
    private readonly IMauiContext _mauiContext;

    private TMauiView? _mauiView;
    private AView? _platformView;
    private Type? _boundViewType;

    private AView? _fallbackView;
    private ContentMode _contentMode = ContentMode.None;

    private int _layoutRetryCount;

    protected FrameLayout Container => _container;

    protected TMauiView? MauiView => _mauiView;

    protected AView? PlatformView => _platformView;

    protected AView? FallbackView => _fallbackView;
    
    protected MauiFormViewHolderBase(FrameLayout container, IMauiContext mauiContext)
        : base(container)
    {
        _container = container;
        _mauiContext = mauiContext;
    }

    public virtual void Unbind()
    {
        if (_mauiView != null)
        {
            OnUnbind(_mauiView);
            _mauiView.BindingContext = null;
        }

        if (_fallbackView != null)
        {
            OnFallbackUnbind(_fallbackView);
        }
    }

    

    protected bool TryBindMauiView(object bindingContext, Type? viewType)
    {
        if (viewType == null)
        {
            return false;
        }

        BindMauiView(bindingContext, viewType);
        return true;
    }

    protected void BindMauiView(object bindingContext, Type viewType)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(viewType);

        EnsureMauiView(viewType);
        EnsureMauiContentAttached();

        if (_mauiView == null)
        {
            return;
        }

        OnBeforeBind(_mauiView, bindingContext);

        _mauiView.BindingContext = bindingContext;

        OnAfterBind(_mauiView, bindingContext);

        _contentMode = ContentMode.Maui;
        MeasureAndArrange();
    }

    protected void ShowFallback(AView fallbackView)
    {
        ArgumentNullException.ThrowIfNull(fallbackView);

        if (ReferenceEquals(_fallbackView, fallbackView) && _contentMode == ContentMode.Fallback)
        {
            return;
        }

        DetachCurrentContent();

        _fallbackView = fallbackView;
        _container.AddView(
            _fallbackView,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            )
        );

        _contentMode = ContentMode.Fallback;
    }

    protected virtual void OnBeforeBind(TMauiView view, object bindingContext) { }

    protected virtual void OnAfterBind(TMauiView view, object bindingContext) { }

    protected virtual void OnUnbind(TMauiView mauiView) { }

    protected virtual void OnFallbackUnbind(AView fallbackView) { }

    protected virtual FrameLayout.LayoutParams CreateLayoutParams(int height) =>
        new(ViewGroup.LayoutParams.MatchParent, height);

    protected void RebindLayout()
    {
        MeasureAndArrange();
    }

    protected virtual AView CreatePlatformView(TMauiView mauiView)
    {
        return mauiView.ToPlatform(_mauiContext);
    }

    protected void ClearCachedFallback()
    {
        if (_fallbackView == null)
        {
            return;
        }

        if (_fallbackView.Parent == _container)
        {
            _container.RemoveView(_fallbackView);
        }

        _fallbackView = null;

        if (_contentMode == ContentMode.Fallback)
        {
            _contentMode = ContentMode.None;
        }
    }

    protected void ClearAllCachedContent()
    {
        if (_platformView?.Parent == _container)
        {
            _container.RemoveView(_platformView);
        }

        if (_fallbackView?.Parent == _container)
        {
            _container.RemoveView(_fallbackView);
        }

        if (_mauiView != null)
        {
            _mauiView.BindingContext = null;
        }

        _mauiView = null;
        _platformView = null;
        _boundViewType = null;
        _fallbackView = null;
        _contentMode = ContentMode.None;
    }

    private void EnsureMauiView(Type viewType)
    {
        if (_mauiView != null && _platformView != null && _boundViewType == viewType)
        {
            return;
        }

        if (_platformView?.Parent == _container)
        {
            _container.RemoveView(_platformView);
        }

        _mauiView = null;
        _platformView = null;
        _boundViewType = null;

        var created = Activator.CreateInstance(viewType);
        if (created is not TMauiView mauiView)
        {
            throw new InvalidOperationException(
                $"Type '{viewType.FullName}' must inherit from '{typeof(TMauiView).FullName}'."
            );
        }

        _mauiView = mauiView;
        _platformView = CreatePlatformView(mauiView);
        _boundViewType = viewType;
    }

    private void EnsureMauiContentAttached()
    {
        if (_platformView == null)
        {
            return;
        }

        if (_contentMode == ContentMode.Maui && _platformView.Parent == _container)
        {
            return;
        }

        DetachCurrentContent();

        _container.AddView(
            _platformView,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            )
        );
    }

    private void DetachCurrentContent()
    {
        if (_platformView?.Parent == _container)
        {
            _container.RemoveView(_platformView);
        }

        if (_fallbackView?.Parent == _container)
        {
            _container.RemoveView(_fallbackView);
        }

        _contentMode = ContentMode.None;
    }

    private void MeasureAndArrange()
    {
        if (_mauiView == null || _platformView == null || _contentMode != ContentMode.Maui)
        {
            return;
        }

        var width = _container.Width - _container.PaddingLeft - _container.PaddingRight;
        if (width <= 0)
        {
            if (_layoutRetryCount >= MaxLayoutRetries)
            {
                return;
            }

            _layoutRetryCount++;
            _container.Post(RebindLayout);
            return;
        }

        _layoutRetryCount = 0;
        var measured = _mauiView.Measure(width, double.PositiveInfinity);
        var height = (int)Math.Ceiling(measured.Height);

        _mauiView.Arrange(new Rect(0, 0, width, height));

        _platformView.LayoutParameters = CreateLayoutParams(height);
        _platformView.RequestLayout();
    }

    private enum ContentMode
    {
        None = 0,
        Maui = 1,
        Fallback = 2,
    }
}
