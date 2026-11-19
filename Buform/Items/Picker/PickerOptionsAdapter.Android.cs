using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

internal sealed class PickerOptionsAdapter : RecyclerView.Adapter
{
    private readonly int _layoutId;
    private readonly Action<IPickerOptionFormItem> _onClick;

    private IReadOnlyList<PickerOptionState> _options;

    public override int ItemCount => _options.Count;

    public PickerOptionsAdapter(int layoutId, Action<IPickerOptionFormItem> onClick)
    {
        ArgumentNullException.ThrowIfNull(onClick);

        _layoutId = layoutId;
        _onClick = onClick;

        _options = [];
    }

    public void SetOptions(IReadOnlyList<PickerOptionState> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var callback = new DiffCallback(_options, options);
        var result = DiffUtil.CalculateDiff(callback);

        _options = options;

        result.DispatchUpdatesTo(this);
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var inflater = LayoutInflater.From(parent.Context)
            ?? throw new InvalidOperationException("The parent view has no layout inflater.");

        var view = inflater.Inflate(_layoutId, parent, false)
            ?? throw new InvalidOperationException("The picker option layout is not inflated.");

        return new PickerOptionViewHolder(view, _onClick);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var optionHolder = (PickerOptionViewHolder)holder;

        optionHolder.Bind(_options[position]);
    }

    private sealed class DiffCallback : DiffUtil.Callback
    {
        private readonly IReadOnlyList<PickerOptionState> _oldOptions;
        private readonly IReadOnlyList<PickerOptionState> _newOptions;

        public override int OldListSize => _oldOptions.Count;

        public override int NewListSize => _newOptions.Count;

        public DiffCallback(
            IReadOnlyList<PickerOptionState> oldOptions,
            IReadOnlyList<PickerOptionState> newOptions
        )
        {
            _oldOptions = oldOptions;
            _newOptions = newOptions;
        }

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            return ReferenceEquals(
                _oldOptions[oldItemPosition].Option,
                _newOptions[newItemPosition].Option
            );
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            var oldOption = _oldOptions[oldItemPosition];
            var newOption = _newOptions[newItemPosition];

            return oldOption.IsPicked == newOption.IsPicked
                && string.Equals(oldOption.Text, newOption.Text, StringComparison.Ordinal);
        }
    }
}
