using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public class TextFormViewHolder : FormViewHolder<ITextFormItem>
{
    private Button? _button;

    public TextFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public TextFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnButtonClick(object? sender, EventArgs e) { }

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

        var isReadOnly = Data?.IsReadOnly ?? true;

        _button.Enabled = !isReadOnly;
    }

    protected virtual void UpdateLabel()
    {
        if (_button == null)
        {
            return;
        }

        _button.Text = Data?.Value.ToString();
    }

    protected virtual void UpdateInputType()
    {
        if (_button == null)
        {
            return;
        }

        // TODO: Update button appearance.
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateInputType();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Data.IsReadOnly):
                UpdateReadOnlyState();
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
