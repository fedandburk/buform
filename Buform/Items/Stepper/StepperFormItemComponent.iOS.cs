// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class StepperFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<StepperFormItem, StepperFormCell>();
    }
}
