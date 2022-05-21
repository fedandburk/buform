using System;
using Foundation;
using UIKit;

namespace Buform
{
    [Preserve(AllMembers = true)]
    [Register(nameof(ListFormCell))]
    public class ListFormCell : FormCell<IListFormItem>
    {
        protected virtual UILabel? Label { get; set; }

        public ListFormCell()
        {
            /* Required constructor */
        }

        public ListFormCell(IntPtr handle) : base(handle)
        {
            /* Required constructor */
        }

        protected override void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.Default;

            Label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.PreferredBody,
                TextColor = UIColor.Label
            };

            ContentView.AddSubviews(Label);

            ContentView.AddConstraints(new[]
            {
                Label.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 11),
                Label.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -11),
                Label.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 16),
                Label.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -16)
            });
        }

        protected virtual void UpdateValue()
        {
            if (Label == null)
            {
                return;
            }

            Label.Text = Item?.FormattedValue;
        }

        protected override void OnItemSet()
        {
            UpdateValue();
        }

        protected override void OnItemPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Item.FormattedValue):
                    UpdateValue();
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Label?.Dispose();
                Label = null;
            }

            base.Dispose(disposing);
        }
    }
}