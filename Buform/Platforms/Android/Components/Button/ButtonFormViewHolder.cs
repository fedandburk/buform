using Android.Runtime;
using Android.Views;
using Fedandburk.Common.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
public class ButtonFormViewHolder : FormViewHolder<ButtonFormItem>
{
    private Button? _button;

    public ButtonFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public ButtonFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnButtonClick(object? sender, EventArgs e)
    {
        Item?.Value.SafeExecute();
    }

    protected override void Initialize()
    {
        _button = ItemView.FindViewById<Button>(Resource.Id.Button)!;
        _button.Click += OnButtonClick;
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_button == null)
        {
            return;
        }

        var isReadOnly = Item?.IsReadOnly ?? true;

        _button.Enabled = !isReadOnly;
    }

    protected virtual void UpdateLabel()
    {
        if (_button == null)
        {
            return;
        }

        _button.Text = Item?.Label;
    }

    protected virtual void UpdateInputType()
    {
        if (_button == null)
        {
            return;
        }

        // TODO: Update button appearance.
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateInputType();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Item.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(Item.Label):
                UpdateLabel();
                break;
            case nameof(Item.InputType):
                UpdateInputType();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var button = _button;
            if (button != null)
            {
                button.Click -= OnButtonClick;
            }

            _button = null;
        }

        base.Dispose(disposing);
    }
}
