// ReSharper disable UnusedMember.Global

using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Buform;

[Register(nameof(MauiFormHeaderFooterView))]
internal sealed class MauiFormHeaderFooterView : UITableViewHeaderFooterView
{
    private FormHeaderFooterView? _formHeaderFooterView;
    private UIView? _view;
    private CGSize _estimatedSize;

    public MauiFormHeaderFooterView()
    {
        /* Required constructor */
    }

    public MauiFormHeaderFooterView(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public void Initialize(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            Type viewType,
        object bindingContext
    )
    {
        _formHeaderFooterView = (Activator.CreateInstance(viewType) as FormHeaderFooterView)!;
        _formHeaderFooterView.BindingContext = bindingContext;

        _view?.RemoveFromSuperview();
        _view = _formHeaderFooterView.ToPlatform(Application.Current!.Handler!.MauiContext!);

        ContentView.AddSubviews(_view);

        EstimateViewSize();
    }

    public override CGSize SizeThatFits(CGSize size)
    {
        return _estimatedSize;
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        if (_formHeaderFooterView == null || _view == null)
        {
            return;
        }

        EstimateViewSize();

        var bounds = new Rect(0, 0, _estimatedSize.Width, _estimatedSize.Height);

        _formHeaderFooterView.Arrange(bounds);

        _view.Frame = bounds;
    }

    private void EstimateViewSize()
    {
        if (_formHeaderFooterView == null || _view == null)
        {
            return;
        }

        var horizontalMargins = ContentView.LayoutMargins.Left + ContentView.LayoutMargins.Right;
        var width = Bounds.Width - horizontalMargins;

        var measuredSize = _formHeaderFooterView.Measure(width, double.PositiveInfinity);

        var verticalMargins = ContentView.LayoutMargins.Top + ContentView.LayoutMargins.Bottom;
        var height = measuredSize.Height - verticalMargins;

        _estimatedSize = new CGSize(width, Math.Ceiling(height));
    }
}
