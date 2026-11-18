// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class DateTimeFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<DateTimeFormItem, DateTimeFormCell>();
    }
}
