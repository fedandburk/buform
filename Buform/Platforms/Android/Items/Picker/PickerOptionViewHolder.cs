using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextView;

namespace Buform;

public sealed class PickerOptionViewHolder : RecyclerView.ViewHolder
{
    private readonly MaterialTextView _titleView;
    private readonly ImageView _checkmarkView;

    private IPickerOptionFormItem? _item;

    public PickerOptionViewHolder(View itemView, Action<IPickerOptionFormItem> onClick)
        : base(itemView)
    {
        _titleView = itemView.FindViewById<MaterialTextView>(Resource.Id.Title)!;
        _checkmarkView = itemView.FindViewById<ImageView>(Resource.Id.Checkmark)!;

        itemView.Click += (_, _) =>
        {
            if (_item != null)
            {
                onClick(_item);
            }
        };
    }

    public void Bind(IPickerOptionFormItem item, bool isSelected)
    {
        _item = item;
        _titleView.Text = item.FormattedValue ?? string.Empty;
        _checkmarkView.Visibility = isSelected ? ViewStates.Visible : ViewStates.Gone;
        ItemView.Selected = isSelected;
    }
}
