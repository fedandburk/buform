using System.Globalization;
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
        UpdateValue();
        UpdateHint();
        UpdateInputType();
        
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(IMultilineTextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(IMultilineTextInputFormItem.Value):
                UpdateValue();
                break;
            case nameof(IMultilineTextInputFormItem.Placeholder):
                UpdateHint();
                break;
            default:
                UpdateReadOnlyState();
                UpdateValue();
                UpdateHint();
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
    }

    protected virtual void UpdateValue()
    {
        if (_input == null)
        {
            return;
        }

        var value = Data?.FormattedValue ?? Data?.Value?.ToString() ?? string.Empty;

        _input.Text = value;
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

        _input.InputType = type.ToAndroidInputType();

        _input.SetSingleLine(false);
        _input.SetHorizontallyScrolling(false);
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
        if (Data?.Value == null)
        {
            return;
        }

        var text = _input?.Text?.Trim() ?? string.Empty;
        var targetType = Data.Value.GetType();
        
        if (TryConvertText(text, targetType, out var converted))
        {
            //Data.Value = converted;
        }
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

    private bool TryConvertText(string text, Type targetType, out object? result)
    {
        result = null;

        if (targetType == typeof(string))
        {
            result = text;
            return true;
        }

        if (targetType == typeof(int))
        {
            if (
                !int.TryParse(
                    text,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var intValue
                )
            )
                return false;
            result = intValue;
            return true;
        }

        if (targetType != typeof(float))
        {
            return false;
        }

        var normalized = text.Replace(',', '.');

        if (
            !float.TryParse(
                normalized,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var floatValue
            )
        )
            return false;
        result = floatValue;
        return true;
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
