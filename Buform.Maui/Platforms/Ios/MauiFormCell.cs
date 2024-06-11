using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class MauiFormCell : UITableViewCell
{
    private readonly UIView _view;

    private CGSize _estimatedSize;

    public MauiFormCell(Type viewType, object bindingContext)
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        var formItemView = (Activator.CreateInstance(viewType) as FormItemView)!;
        formItemView.BindingContext = bindingContext;

        _view = formItemView.ToPlatform(Application.Current!.Handler!.MauiContext!);

        ContentView.AddSubview(_view);

        EstimateViewSize();
    }

    private void EstimateViewSize()
    {
        // var width = Bounds.Width;
        //
        // var request = _formItemView.Measure(
        //     width,
        //     double.PositiveInfinity,
        //     MeasureFlags.IncludeMargins
        // );
        //
        // _estimatedSize = new CGSize(width, Math.Ceiling(request.Request.Height));
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
        // Microsoft.Maui.Controls.Compatibility.Layout.LayoutChildIntoBoundingRegion(
        //     _formItemView,
        //     bounds
        // );
        //
        // _viewHandler.PlatformView!.Frame = bounds.ToRectangleF();
    }
}
