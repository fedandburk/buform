using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(MauiFormHeaderFooterView))]
internal sealed class MauiFormHeaderFooterView : UITableViewHeaderFooterView
{
    private FormHeaderFooterView? _formHeaderFooterView;
    private UIView? _view;
    private CGSize _estimatedSize;

    // ReSharper disable once UnusedMember.Global
    public MauiFormHeaderFooterView()
    {
        /* Required constructor */
    }

    // ReSharper disable once UnusedMember.Global
    public MauiFormHeaderFooterView(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public void Initialize(Type viewType, object bindingContext)
    {
        _formHeaderFooterView = (Activator.CreateInstance(viewType) as FormHeaderFooterView)!;
        _formHeaderFooterView.BindingContext = bindingContext;

        _view = _formHeaderFooterView.ToPlatform(Application.Current!.Handler!.MauiContext!);

        ContentView.AddSubviews(_view);

        EstimateViewSize();
    }

    private void EstimateViewSize()
    {
        if (_formHeaderFooterView == null || _view == null)
        {
            return;
        }

        var horizontalMargins = ContentView.LayoutMargins.Left + ContentView.LayoutMargins.Right;
        var width = Bounds.Width - horizontalMargins;

        var request = _formHeaderFooterView.Measure(
            width,
            double.PositiveInfinity,
            MeasureFlags.IncludeMargins
        );

        var verticalMargins = ContentView.LayoutMargins.Top + ContentView.LayoutMargins.Bottom;
        var height = request.Request.Height - verticalMargins;

        _estimatedSize = new CGSize(width, Math.Ceiling(height));
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

        Microsoft.Maui.Controls.Compatibility.Layout.LayoutChildIntoBoundingRegion(
            _formHeaderFooterView,
            bounds
        );

        _view.Frame = bounds.ToRectangleF();
    }
}
