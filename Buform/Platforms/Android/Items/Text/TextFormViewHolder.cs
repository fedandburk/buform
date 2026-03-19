using Android.Runtime;
using Android.Util;
using Android.Views;
using Google.Android.Material.TextView;

namespace Buform;

[Preserve(AllMembers = true)]
public class TextFormViewHolder : FormViewHolder<ITextFormItem>
{
    private MaterialTextView? _textView;

    public TextFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public TextFormViewHolder(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        _textView = ItemView.FindViewById<MaterialTextView>(Resource.Id.Text)!;
        _textView.SetTextSize(ComplexUnitType.Sp, 18);

        ApplyTextColorFromTheme();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;

            case nameof(ITextFormItem.Value):
            case nameof(ITextFormItem.FormattedValue):
                UpdateLabel();
                break;

            default:
                UpdateReadOnlyState();
                UpdateLabel();
                break;
        }
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_textView == null)
        {
            return;
        }

        var isReadOnly = Data?.IsReadOnly ?? true;

        ItemView.Enabled = !isReadOnly;
        ItemView.Clickable = !isReadOnly;
        ItemView.Focusable = !isReadOnly;
        _textView.Enabled = !isReadOnly;
    }

    protected virtual void UpdateLabel()
    {
        if (_textView == null)
        {
            return;
        }

        var value = Data?.FormattedValue ?? Data?.Value?.ToString() ?? string.Empty;

        if (!string.Equals(_textView.Text, value, StringComparison.Ordinal))
        {
            _textView.Text = value;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _textView = null;
        }

        base.Dispose(disposing);
    }

    private void ApplyTextColorFromTheme()
    {
        if (_textView == null)
        {
            return;
        }

        var typedValue = new TypedValue();
        var theme = _textView.Context?.Theme;

        if (
            theme != null
            && theme.ResolveAttribute(Resource.Attribute.colorOnSurface, typedValue, true)
        )
        {
            _textView.SetTextColor(new Android.Graphics.Color(typedValue.Data));
        }
    }
}
