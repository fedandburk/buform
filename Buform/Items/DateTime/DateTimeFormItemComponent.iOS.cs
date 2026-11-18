namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class DateTimeFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<DateTimeFormItem, DateTimeFormCell>();
    }
}
