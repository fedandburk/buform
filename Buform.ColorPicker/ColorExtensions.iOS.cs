using System.Drawing;

namespace Buform;

internal static class ColorExtensions
{
    private const int ColorComponentMaximum = 255;

    public static UIColor ToColor(this Color color)
    {
        return UIColor.FromRGBA(color.R, color.G, color.B, color.A);
    }

    public static Color ToColor(this UIColor color)
    {
        ArgumentNullException.ThrowIfNull(color);

        color.GetRGBA(out var red, out var green, out var blue, out var alpha);

        return Color.FromArgb(
            (int)(alpha * ColorComponentMaximum),
            (int)(red * ColorComponentMaximum),
            (int)(green * ColorComponentMaximum),
            (int)(blue * ColorComponentMaximum)
        );
    }
}
