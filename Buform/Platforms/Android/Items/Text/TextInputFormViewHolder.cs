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
    private bool _isUpdatingText;

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
        UpdateHint();
        UpdateInputType();
        UpdateHelperText();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;

            case nameof(ITextInputFormItem.Value):
            case nameof(ITextInputFormItem.FormattedValue):
                UpdateValue();
                break;

            case nameof(ITextInputFormItem.Placeholder):
                UpdateHint();
                break;

            case nameof(ITextInputFormItem.Label):
                UpdateHelperText();
                break;

            case nameof(ITextInputFormItem.InputType):
                UpdateInputType();
                break;

            default:
                UpdateReadOnlyState();
                UpdateHint();
                UpdateInputType();
                UpdateHelperText();
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
        _input.InputType = type.ToAndroidInputType();

        if (Data?.IsSecured == true)
        {
            _input.InputType |= InputTypes.TextVariationPassword;
        }
    }

    protected virtual void UpdateHelperText()
    {
        if (_layout == null)
        {
            return;
        }

        var helper = Data?.Label;

        if (!string.IsNullOrEmpty(helper))
        {
            _layout.HelperTextEnabled = true;
            _layout.HelperText = helper;
        }
        else
        {
            _layout.HelperTextEnabled = false;
            _layout.HelperText = null;
        }
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
        if (_isUpdatingText || Data == null || _input == null)
        {
            return;
        }

        var text = _input.Text ?? string.Empty;
        var currentValue = Data.Value;

        if (currentValue == null)
        {
            Data.SetValue(text);
            return;
        }

        var targetType = currentValue.GetType();

        if (!TryConvertText(text, targetType, out var converted))
        {
            return;
        }

        if (Equals(currentValue, converted))
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

    private bool TryConvertText(string text, Type targetType, out object? result)
    {
        result = null;

        if (targetType == typeof(string))
        {
            result = text;
            return true;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
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
            {
                return false;
            }

            result = intValue;
            return true;
        }

        if (targetType == typeof(float))
        {
            var normalized = text.Replace(',', '.');

            if (
                !float.TryParse(
                    normalized,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var floatValue
                )
            )
            {
                return false;
            }

            result = floatValue;
            return true;
        }

        if (targetType == typeof(double))
        {
            var normalized = text.Replace(',', '.');

            if (
                !double.TryParse(
                    normalized,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var doubleValue
                )
            )
            {
                return false;
            }

            result = doubleValue;
            return true;
        }

        if (targetType == typeof(decimal))
        {
            var normalized = text.Replace(',', '.');

            if (
                !decimal.TryParse(
                    normalized,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var decimalValue
                )
            )
            {
                return false;
            }

            result = decimalValue;
            return true;
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
