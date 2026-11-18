// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class SliderFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<SliderFormItem, SliderFormCell>();
    }
}
