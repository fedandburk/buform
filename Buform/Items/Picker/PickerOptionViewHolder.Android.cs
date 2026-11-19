using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

internal sealed class PickerOptionViewHolder : RecyclerView.ViewHolder
{
    private readonly CompoundButton _optionView;
    private readonly Action<IPickerOptionFormItem> _onClick;

    private IPickerOptionFormItem? _option;

    public PickerOptionViewHolder(View itemView, Action<IPickerOptionFormItem> onClick)
        : base(itemView)
    {
        ArgumentNullException.ThrowIfNull(onClick);

        _onClick = onClick;

        _optionView = (CompoundButton)itemView;
        _optionView.Click += OnOptionClick;
    }

    private void OnOptionClick(object? sender, EventArgs e)
    {
        if (_option == null)
        {
            return;
        }

        _onClick(_option);
    }

    public void Bind(PickerOptionState state)
    {
        _option = state.Option;

        _optionView.Text = state.Text;
        _optionView.Checked = state.IsPicked;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _optionView.Click -= OnOptionClick;
        }

        base.Dispose(disposing);
    }
}
