using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.TextField;

namespace Buform;

[Preserve(AllMembers = true)]
public class TextMultilineFormViewHolder : FormViewHolder<IMultilineTextInputFormItem>
{
    private TextInputLayout? _layout;
    private TextInputEditText? _input;
    private bool _isUpdatingText;

    public TextMultilineFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public TextMultilineFormViewHolder(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        _layout = ItemView.FindViewById<TextInputLayout>(Resource.Id.InputLayout);
        _input = ItemView.FindViewById<TextInputEditText>(Resource.Id.Input)!;

        ApplyTheme();
        AttachListeners();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateHint();
        UpdateInputType();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(IMultilineTextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;

            case nameof(IMultilineTextInputFormItem.Value):
            case nameof(IMultilineTextInputFormItem.FormattedValue):
                UpdateValue();
                break;

            case nameof(IMultilineTextInputFormItem.Placeholder):
                UpdateHint();
                break;

            case nameof(IMultilineTextInputFormItem.InputType):
                UpdateInputType();
                break;

            default:
                UpdateReadOnlyState();
                UpdateHint();
                UpdateInputType();
                UpdateValue();
                break;
        }
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_input == null)
        {
            return;
        }

        var isReadOnly = Data?.IsReadOnly ?? true;

        _input.Enabled = !isReadOnly;
        _input.Focusable = !isReadOnly;
        _input.FocusableInTouchMode = !isReadOnly;
        _input.Clickable = !isReadOnly;
        _input.LongClickable = !isReadOnly;
    }

    protected virtual void UpdateValue()
    {
        if (_input == null)
        {
            return;
        }

        var value = Data?.FormattedValue ?? Data?.Value?.ToString() ?? string.Empty;

        if (string.Equals(_input.Text, value, StringComparison.Ordinal))
        {
            return;
        }

        _isUpdatingText = true;
        try
        {
            _input.Text = value;
            _input.SetSelection(_input.Text?.Length ?? 0);
        }
        finally
        {
            _isUpdatingText = false;
        }
    }

    protected virtual void UpdateHint()
    {
        if (_input == null)
        {
            return;
        }

        _input.Hint = Data?.Placeholder ?? string.Empty;

        if (_layout != null)
        {
            _layout.HintEnabled = false;
        }
    }

    protected virtual void UpdateInputType()
    {
        if (_input == null)
        {
            return;
        }

        var type = Data?.InputType ?? TextInputType.Default;

        _input.InputType = type.ToAndroidInputType() | InputTypes.TextFlagMultiLine;
        _input.SetSingleLine(false);
        _input.SetHorizontallyScrolling(false);
        _input.SetMaxLines(int.MaxValue);
    }

    private void AttachListeners()
    {
        if (_input == null)
        {
            return;
        }

        _input.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdatingText || Data == null)
        {
            return;
        }

        var text = _input?.Text;

        if (
            string.Equals(
                Data.FormattedValue ?? Data.Value?.ToString(),
                text,
                StringComparison.Ordinal
            )
        )
        {
            return;
        }

        Data.SetValue(text);
    }

    private void ApplyTheme()
    {
        if (_input == null)
        {
            return;
        }

        var typedValue = new Android.Util.TypedValue();
        var theme = _input.Context?.Theme;

        if (
            theme != null
            && theme.ResolveAttribute(Resource.Attribute.colorOnSurface, typedValue, true)
        )
        {
            _input.SetTextColor(new Android.Graphics.Color(typedValue.Data));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _input != null)
        {
            _input.TextChanged -= OnTextChanged;
            _input = null;
            _layout = null;
        }

        base.Dispose(disposing);
    }
}
