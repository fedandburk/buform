using System;
using Foundation;

namespace Buform
{
    [Preserve(AllMembers = true)]
    public sealed class TextFormGroupFooter : FormHeaderFooter<TextFormGroup>
    {
        public TextFormGroupFooter()
        {
            /* Required constructor */
        }

        public TextFormGroupFooter(IntPtr handle) : base(handle)
        {
            /* Required constructor */
        }

        private void UpdateLabel()
        {
            var tableView = GetTableView();

            tableView?.BeginUpdates();

            TextLabel.Text = Group?.FooterLabel;

            tableView?.EndUpdates();
        }

        protected override void OnGroupSet()
        {
            UpdateLabel();
        }

        protected override void OnGroupPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Group.FooterLabel):
                    UpdateLabel();
                    break;
            }
        }
    }
}