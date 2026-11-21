// ReSharper disable UnusedMember.Global

using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Buform;

[Register(nameof(MauiFormCell))]
internal sealed class MauiFormCell : UITableViewCell
{
    private FormItemView? _formItemView;
    private UIView? _view;
    private CGSize _estimatedSize;

    public MauiFormCell()
    {
        /* Required constructor */
    }

    public MauiFormCell(NativeHandle handle)
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
        SelectionStyle = UITableViewCellSelectionStyle.None;

        _formItemView = (Activator.CreateInstance(viewType) as FormItemView)!;
        _formItemView.BindingContext = bindingContext;

        _view?.RemoveFromSuperview();
        _view = _formItemView.ToPlatform(Application.Current!.Handler!.MauiContext!);

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

        if (_formItemView == null || _view == null)
        {
            return;
        }

        EstimateViewSize();

        var bounds = new Rect(0, 0, _estimatedSize.Width, _estimatedSize.Height);

        _formItemView.Arrange(bounds);

        _view.Frame = bounds;
    }

    private void EstimateViewSize()
    {
        if (_formItemView == null || _view == null)
        {
            return;
        }

        var width = Bounds.Width;

        var measuredSize = _formItemView.Measure(width, double.PositiveInfinity);

        var height = Math.Ceiling(measuredSize.Height);

        _estimatedSize = new CGSize(width, height);
    }
}
