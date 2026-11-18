// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class SegmentsFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<ISegmentsFormItem, SegmentsFormCell>();
    }
}
