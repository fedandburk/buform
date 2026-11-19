namespace Buform;

internal static class FormFonts
{
    private const float DescriptorPointSize = 0;

    public static UIFont GetBody()
    {
        var font = UIFont.PreferredBody;

        if (font == null)
        {
            throw new InvalidOperationException("The system has no body font.");
        }

        return font;
    }

    public static UIFont GetBoldBody()
    {
        var bodyFont = GetBody();

        var descriptor = bodyFont.FontDescriptor.CreateWithTraits(
            UIFontDescriptorSymbolicTraits.Bold
        );

        if (descriptor == null)
        {
            throw new InvalidOperationException("The body font has no bold variant.");
        }

        var font = UIFont.FromDescriptor(descriptor, DescriptorPointSize);

        if (font == null)
        {
            throw new InvalidOperationException("The bold body font cannot be created.");
        }

        return font;
    }
}
