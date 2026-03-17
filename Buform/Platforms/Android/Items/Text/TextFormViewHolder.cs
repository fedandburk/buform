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
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public TextFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        _textView = ItemView.FindViewById<MaterialTextView>(Resource.Id.Text)!;

        _textView.SetTextSize(ComplexUnitType.Sp, 18);

        ApplyTextColorFromTheme();
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_textView == null)
            return;

        var isReadOnly = Data?.IsReadOnly ?? true;

        ItemView.Enabled = !isReadOnly;
        ItemView.Clickable = !isReadOnly;
        ItemView.Focusable = !isReadOnly;
        _textView.Enabled = !isReadOnly;
    }

    protected virtual void UpdateLabel()
    {
        if (_textView == null)
            return;

        _textView.Text = Data?.FormattedValue ?? string.Empty;
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
                UpdateLabel();
                break;

            case null:
            case "":
                UpdateReadOnlyState();
                UpdateLabel();
                break;
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
            return;

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
