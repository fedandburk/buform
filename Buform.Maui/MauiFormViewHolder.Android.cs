using System.Diagnostics.CodeAnalysis;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using MView = Microsoft.Maui.Controls.View;

namespace Buform;

internal abstract class MauiFormViewHolder<TMauiView> : RecyclerView.ViewHolder
    where TMauiView : MView
{
    private const int MaxLayoutRetries = 3;

    private readonly IMauiContext _mauiContext;

    private FormViewHolder? _nativeViewHolder;
    private TMauiView? _mauiView;
    private AView? _platformView;
    private Type? _mauiViewType;
    private int _layoutRetryCount;

    protected FrameLayout Container { get; }

    public AView? ContentView => Container.ChildCount > 0 ? Container.GetChildAt(0) : null;

    protected MauiFormViewHolder(FrameLayout container, IMauiContext mauiContext)
        : base(container)
    {
        ArgumentNullException.ThrowIfNull(mauiContext);

        Container = container;
        _mauiContext = mauiContext;
    }

    public virtual void Unbind()
    {
        _nativeViewHolder?.Dispose();
        _nativeViewHolder = null;

        if (_mauiView != null)
        {
            _mauiView.BindingContext = null;
        }

        Container.RemoveAllViews();
    }

    protected void BindMauiView(
        object bindingContext,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            Type viewType
    )
    {
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(viewType);

        if (_mauiView == null || _mauiViewType != viewType)
        {
            _mauiView = Activator.CreateInstance(viewType) as TMauiView;

            if (_mauiView == null)
            {
                throw new InvalidOperationException(
                    $"Type '{viewType.FullName}' must inherit from '{typeof(TMauiView).FullName}'."
                );
            }

            _platformView = _mauiView.ToPlatform(_mauiContext);
            _mauiViewType = viewType;
        }

        _mauiView.BindingContext = bindingContext;

        ShowView(WrapPlatformView(_platformView!));

        MeasureAndArrange();
    }

    protected void BindNativeView(int viewType, IDisposable data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (
            !FormPlatform.TryGetViewHolderAndResourceId(
                viewType,
                out var viewHolderType,
                out var resourceId
            )
        )
        {
            throw new InvalidOperationException(
                $"View type '{viewType}' has no registered view holder."
            );
        }

        var inflater = LayoutInflater.From(Container.Context)
            ?? throw new InvalidOperationException("The container has no layout inflater.");

        var view = inflater.Inflate(resourceId!.Value, Container, false)
            ?? throw new InvalidOperationException("The view holder layout is not inflated.");

        _nativeViewHolder = FormPlatform.CreateViewHolder(viewHolderType!, view);
        _nativeViewHolder.Initialize(data);

        ShowView(view);
    }

    protected virtual AView WrapPlatformView(AView platformView)
    {
        return platformView;
    }

    protected virtual int GetHorizontalInset()
    {
        return 0;
    }

    protected void ShowView(AView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view.Parent == Container)
        {
            return;
        }

        Container.RemoveAllViews();
        Container.AddView(
            view,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            )
        );
    }

    private void MeasureAndArrange()
    {
        if (_mauiView == null || _platformView == null)
        {
            return;
        }

        var width =
            Container.Width - Container.PaddingLeft - Container.PaddingRight - GetHorizontalInset();

        if (width <= 0)
        {
            if (_layoutRetryCount >= MaxLayoutRetries)
            {
                return;
            }

            _layoutRetryCount++;

            Container.Post(MeasureAndArrange);

            return;
        }

        _layoutRetryCount = 0;

        var context = Container.Context
            ?? throw new InvalidOperationException("The container has no context.");

        var mauiWidth = context.FromPixels(width);
        var size = _mauiView.Measure(mauiWidth, double.PositiveInfinity);

        _mauiView.Arrange(new Rect(0, 0, mauiWidth, size.Height));

        var height = (int)Math.Ceiling(context.ToPixels(size.Height));

        _platformView.LayoutParameters = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            height
        );
        _platformView.RequestLayout();
    }
}
