namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class SliderFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<SliderFormItem, SliderFormCell>();
    }
}
