// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class ColorPickerFormComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<ColorPickerFormItem, ColorPickerFormCell>();
    }
}
