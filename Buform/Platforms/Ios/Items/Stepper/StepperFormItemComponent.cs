namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class StepperFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<StepperFormItem, StepperFormCell>();
    }
}
