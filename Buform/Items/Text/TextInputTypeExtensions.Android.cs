using Android.Text;

namespace Buform;

internal static class TextInputTypeExtensions
{
    public static InputTypes ToInputTypes(this TextInputType inputType)
    {
        return inputType switch
        {
            TextInputType.Default => InputTypes.ClassText,
            TextInputType.NumberAndPunctuation => InputTypes.ClassText
                | InputTypes.TextFlagNoSuggestions,
            TextInputType.Number => InputTypes.ClassNumber,
            TextInputType.Decimal => InputTypes.ClassNumber | InputTypes.NumberFlagDecimal,
            TextInputType.Phone => InputTypes.ClassPhone,
            TextInputType.Url => InputTypes.ClassText | InputTypes.TextVariationUri,
            TextInputType.EmailAddress => InputTypes.ClassText
                | InputTypes.TextVariationEmailAddress,
            _ => throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null),
        };
    }
}
