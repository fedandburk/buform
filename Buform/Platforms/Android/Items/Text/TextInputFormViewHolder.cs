using System.Globalization;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.TextField;

namespace Buform;

[Preserve(AllMembers = true)]
public class TextInputFormViewHolder : FormViewHolder<ITextInputFormItem>
{
    private TextInputLayout? _layout;
    private TextInputEditText? _input;

    public TextInputFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public TextInputFormViewHolder(View itemView)
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
        UpdateHelperText();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;

            case nameof(ITextInputFormItem.Value):
                UpdateValue();
                break;

            case nameof(ITextInputFormItem.Placeholder):
                UpdateHint();
                break;

            case nameof(ITextInputFormItem.Label):
                UpdateHelperText();
                break;

            case null:
            case "":
                UpdateReadOnlyState();
                UpdateValue();
                UpdateHint();
                break;
        }
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_input == null)
            return;

        var isReadOnly = Data?.IsReadOnly ?? true;

        _input.Enabled = !isReadOnly;
        _input.Focusable = !isReadOnly;
        _input.FocusableInTouchMode = !isReadOnly;
        _input.Clickable = !isReadOnly;
    }

    protected virtual void UpdateValue()
    {
        if (_input == null)
            return;

        var value = Data?.FormattedValue ?? Data?.Value?.ToString() ?? string.Empty;

        if (_input.Text != value)
            _input.Text = value;
    }

    protected virtual void UpdateHint()
    {
        if (_input == null)
            return;

        _input.Hint = Data?.Placeholder ?? string.Empty;

        if (_layout != null)
            _layout.HintEnabled = false;
    }

    protected virtual void UpdateInputType()
    {
        if (_input == null)
            return;

        var type = Data?.InputType ?? TextInputType.Default;

        _input.InputType = type.ToAndroidInputType();
    }

    protected virtual void UpdateHelperText()
    {
        if (_layout == null)
            return;

        var helper = Data?.Label;

        if (!string.IsNullOrEmpty(helper))
        {
            _layout.HelperTextEnabled = true;
            _layout.HelperText = helper;
        }
        else
        {
            _layout.HelperTextEnabled = false;
        }
    }

    private void AttachListeners()
    {
        if (_input == null)
            return;

        _input.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (Data?.Value == null)
            return;

        var text = _input?.Text?.Trim() ?? string.Empty;
        var targetType = Data.Value.GetType();

        if (TryConvertText(text, targetType, out var converted))
        {
            Data.Value = converted;
        }
    }

    private void ApplyTheme()
    {
        if (_input == null)
            return;

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
                int.TryParse(
                    text,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var intValue
                )
            )
            {
                result = intValue;
                return true;
            }

            return false;
        }

        if (targetType == typeof(float))
        {
            var normalized = text.Replace(',', '.');

            if (
                float.TryParse(
                    normalized,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var floatValue
                )
            )
            {
                result = floatValue;
                return true;
            }

            return false;
        }

        return false;
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

public static class TextInputTypeExtensions
{
    public static InputTypes ToAndroidInputType(this TextInputType type)
    {
        return type switch
        {
            TextInputType.Default => InputTypes.ClassText,

            TextInputType.NumberAndPunctuation
                => InputTypes.ClassText | InputTypes.TextFlagNoSuggestions,

            TextInputType.Number => InputTypes.ClassNumber,

            TextInputType.Decimal => InputTypes.ClassNumber | InputTypes.NumberFlagDecimal,

            TextInputType.Phone => InputTypes.ClassPhone,

            TextInputType.Url => InputTypes.ClassText | InputTypes.TextVariationUri,

            TextInputType.EmailAddress
                => InputTypes.ClassText | InputTypes.TextVariationEmailAddress,

            _ => InputTypes.ClassText
        };
    }
}
