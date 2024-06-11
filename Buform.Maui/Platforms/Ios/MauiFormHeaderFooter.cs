using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class MauiFormHeaderFooter : UITableViewHeaderFooterView
{
    private readonly UIView _view;

    private CGSize _estimatedSize;

    public MauiFormHeaderFooter(Type viewType, object bindingContext)
        : base((NSString)viewType.Name)
    {
        var formHeaderFooter = (Activator.CreateInstance(viewType) as FormHeaderFooterView)!;

        formHeaderFooter.BindingContext = bindingContext;

        _view = formHeaderFooter.ToPlatform(Application.Current!.Handler!.MauiContext!);

        ContentView.AddSubview(_view);

        EstimateViewSize();
    }

    private void EstimateViewSize()
    {
        // var horizontalMargins = LayoutMargins.Left + LayoutMargins.Right;
        // var width = Bounds.Width - horizontalMargins;
        //
        // var request = _view.Measure(width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
        //
        // var verticalMargins = LayoutMargins.Top + LayoutMargins.Bottom;
        //
        // _estimatedSize = new CGSize(width, Math.Ceiling(request.Request.Height - verticalMargins));
    }

    public override CGSize SizeThatFits(CGSize size)
    {
        return _estimatedSize;
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        EstimateViewSize();

        // var bounds = new Rect(0, 0, _estimatedSize.Width, _estimatedSize.Height);
        //
        // Microsoft.Maui.Controls.Compatibility.Layout.LayoutChildIntoBoundingRegion(_view, bounds);
        //
        // _viewHandler.PlatformView!.Frame = bounds.ToRectangleF();
    }
}
