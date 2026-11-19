namespace Buform;

public static class ColorPickerFormPlatform
{
    public static void Register()
    {
        FormPlatform.RegisterItem<ColorPickerFormItem, ColorPickerFormViewHolder>(
            Resource.Layout.FormItemColorPickerLayout
        );
    }
}
