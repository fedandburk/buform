using System;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Buform;

internal sealed class TableViewHeaderFooterView : UITableViewHeaderFooterView
{
    private readonly FormsHeaderFooterView _formsHeaderFooterView;
    private readonly ViewHandler _viewHandler;

    private CGSize _estimatedSize;

    public TableViewHeaderFooterView(Type viewType, object bindingContext)
        : base((NSString)viewType.Name)
    {
        _formsHeaderFooterView = (Activator.CreateInstance(viewType) as FormsHeaderFooterView)!;

        _formsHeaderFooterView.BindingContext = bindingContext;

        // TODO: Resolve handler for FormsHeaderFooterView.
        _viewHandler = default;

        ContentView.AddSubview(_viewHandler.PlatformView!);

        EstimateViewSize();
    }

    private void EstimateViewSize()
    {
        var horizontalMargins = LayoutMargins.Left + LayoutMargins.Right;
        var width = Bounds.Width - horizontalMargins;

        var request = _formsHeaderFooterView.Measure(
            width,
            double.PositiveInfinity,
            MeasureFlags.IncludeMargins
        );

        var verticalMargins = LayoutMargins.Top + LayoutMargins.Bottom;

        _estimatedSize = new CGSize(width, Math.Ceiling(request.Request.Height - verticalMargins));
    }

    public override CGSize SizeThatFits(CGSize size)
    {
        return _estimatedSize;
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        EstimateViewSize();

        var bounds = new Rect(0, 0, _estimatedSize.Width, _estimatedSize.Height);

        Microsoft.Maui.Controls.Compatibility.Layout.LayoutChildIntoBoundingRegion(
            _formsHeaderFooterView,
            bounds
        );

        _viewHandler.PlatformView!.Frame = bounds.ToRectangleF();
    }
}
