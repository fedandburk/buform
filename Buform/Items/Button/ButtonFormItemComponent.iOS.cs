// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class ButtonFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<ButtonFormItem, ButtonFormCell>();
    }
}
