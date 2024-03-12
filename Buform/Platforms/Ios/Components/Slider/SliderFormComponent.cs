namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class SliderFormComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<SliderFormItem, SliderFormCell>();
    }
}
