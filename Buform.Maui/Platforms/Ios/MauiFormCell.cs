using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(MauiFormCell))]
internal sealed class MauiFormCell : UITableViewCell
{
    private FormItemView? _formItemView;
    private UIView? _view;
    private CGSize _estimatedSize;

    // ReSharper disable once UnusedMember.Global
    public MauiFormCell()
    {
        /* Required constructor */
    }

    // ReSharper disable once UnusedMember.Global
    public MauiFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public void Initialize(Type viewType, object bindingContext)
    {
        SelectionStyle = UITableViewCellSelectionStyle.None;

        _formItemView = (Activator.CreateInstance(viewType) as FormItemView)!;
        _formItemView.BindingContext = bindingContext;

        _view?.RemoveFromSuperview();
        _view = _formItemView.ToPlatform(Application.Current!.Handler!.MauiContext!);

        ContentView.AddSubviews(_view);

        EstimateViewSize();
    }

    private void EstimateViewSize()
    {
        if (_formItemView == null || _view == null)
        {
            return;
        }

        var width = Bounds.Width;

        var request = _formItemView.Measure(
            width,
            double.PositiveInfinity,
            MeasureFlags.IncludeMargins
        );

        var height = Math.Ceiling(request.Request.Height);

        _estimatedSize = new CGSize(width, height);
    }

    public override CGSize SizeThatFits(CGSize size)
    {
        return _estimatedSize;
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        if (_formItemView == null || _view == null)
        {
            return;
        }

        EstimateViewSize();

        var bounds = new Rect(0, 0, _estimatedSize.Width, _estimatedSize.Height);

        Microsoft.Maui.Controls.Compatibility.Layout.LayoutChildIntoBoundingRegion(
            _formItemView,
            bounds
        );

        _view.Frame = bounds;
    }
}
