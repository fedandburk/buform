namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class SwitchFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<SwitchFormItem, SwitchFormCell>();
    }
}
