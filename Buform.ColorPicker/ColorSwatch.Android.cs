using Android.Content;
using Android.Graphics.Drawables;
using Google.Android.Material.Color;

namespace Buform;

internal static class ColorSwatch
{
    public static Drawable Create(Context context, int color, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(context);

        var resources = context.Resources
            ?? throw new InvalidOperationException("The context has no resources.");

        var strokeWidthId = isSelected
            ? Resource.Dimension.color_picker_selected_swatch_stroke_width
            : Resource.Dimension.color_picker_swatch_stroke_width;
        var strokeColorId = isSelected
            ? Resource.Attribute.colorPrimary
            : Resource.Attribute.colorOutline;

        var strokeWidth = resources.GetDimensionPixelSize(strokeWidthId);
        var strokeColor = MaterialColors.GetColor(context, strokeColorId, nameof(ColorSwatch));

        var drawable = new GradientDrawable();

        drawable.SetShape(ShapeType.Oval);
        drawable.SetColor(color);
        drawable.SetStroke(strokeWidth, new Android.Graphics.Color(strokeColor));

        return drawable;
    }
}
