using CoreGraphics;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Buform;

internal sealed class TableViewCell : UITableViewCell
{
    private readonly FormItemView _formItemView;
    private readonly ViewHandler _viewHandler;

    private CGSize _estimatedSize;

    public TableViewCell(Type viewType, object bindingContext)
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        _formItemView = (Activator.CreateInstance(viewType) as FormItemView)!;

        _formItemView.BindingContext = bindingContext;

        // TODO: Resolve handler for FormItemView.
        _viewHandler = default;

        ContentView.AddSubviews(_viewHandler.PlatformView!);
    }

    private void EstimateViewSize()
    {
        var width = Bounds.Width;

        var request = _formItemView.Measure(
            width,
            double.PositiveInfinity,
            MeasureFlags.IncludeMargins
        );

        _estimatedSize = new CGSize(width, Math.Ceiling(request.Request.Height));
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
            _formItemView,
            bounds
        );

        _viewHandler.PlatformView!.Frame = bounds.ToRectangleF();
    }
}
