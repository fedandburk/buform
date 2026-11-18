// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class SwitchFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<SwitchFormItem, SwitchFormCell>();
    }
}
