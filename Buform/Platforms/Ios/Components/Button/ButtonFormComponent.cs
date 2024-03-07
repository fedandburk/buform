namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class ButtonFormComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<ButtonFormItem, ButtonFormCell>();
    }
}
