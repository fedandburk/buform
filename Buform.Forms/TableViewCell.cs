using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Platform = Xamarin.Forms.Platform.iOS.Platform;

namespace Buform
{
    internal sealed class TableViewCell : UITableViewCell
    {
        private readonly FormsFormCell _formsFormCell;
        private readonly IVisualElementRenderer _renderer;

        private CGSize _estimatedSize;

        public TableViewCell(
            Type viewType,
            object bindingContext
        )
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            _formsFormCell = (Activator.CreateInstance(viewType) as FormsFormCell)!;

            _formsFormCell.BindingContext = bindingContext;

            _renderer = Platform.CreateRenderer(_formsFormCell);

            ContentView.AddSubviews(_renderer.NativeView);
        }

        private void EstimateViewSize()
        {
            var width = Bounds.Width;

            var request = _formsFormCell.Measure(
                width,
                double.PositiveInfinity,
                MeasureFlags.IncludeMargins
            );

            _estimatedSize = new CGSize(
                width,
                Math.Ceiling(request.Request.Height)
            );
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            return _estimatedSize;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            EstimateViewSize();

            var bounds = new Rectangle(
                0,
                0,
                _estimatedSize.Width,
                _estimatedSize.Height
            );

            Layout.LayoutChildIntoBoundingRegion(_formsFormCell, bounds);

            _renderer.NativeView.Frame = bounds.ToRectangleF();
        }
    }
}