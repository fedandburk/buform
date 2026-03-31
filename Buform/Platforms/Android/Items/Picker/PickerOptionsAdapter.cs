using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

public sealed class PickerOptionsAdapter : RecyclerView.Adapter
{
    private readonly IReadOnlyList<IPickerOptionFormItem> _items;
    private readonly Action<IPickerOptionFormItem> _onClick;
    private readonly Func<IPickerOptionFormItem, bool> _isSelected;

    public PickerOptionsAdapter(
        IReadOnlyList<IPickerOptionFormItem> items,
        Action<IPickerOptionFormItem> onClick,
        Func<IPickerOptionFormItem, bool> isSelected
    )
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(onClick);
        ArgumentNullException.ThrowIfNull(isSelected);

        _items = items;
        _onClick = onClick;
        _isSelected = isSelected;
    }

    public override int ItemCount => _items.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = LayoutInflater
            .From(parent.Context)
            ?.Inflate(Resource.Layout.PickerOptionItemLayout, parent, false);

        return view == null
            ? throw new InvalidOperationException("Failed to inflate PickerOptionItemLayout")
            : new PickerOptionViewHolder(view, _onClick);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is not PickerOptionViewHolder optionHolder)
        {
            return;
        }

        var item = _items[position];
        optionHolder.Bind(item, _isSelected(item));
    }
}
