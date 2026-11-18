namespace Buform;

public static class ColorPickerFormPlatform
{
    public static void Register()
    {
        FormPlatform.RegisterItemClass<ColorPickerFormItem, ColorPickerFormCell>();
    }
}
