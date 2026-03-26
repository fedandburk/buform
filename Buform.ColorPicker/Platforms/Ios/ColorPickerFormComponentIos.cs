namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class ColorPickerFormComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<ColorPickerFormItem, ColorPickerFormCell>();
    }
}
