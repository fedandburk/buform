namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class ListFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<IListFormItem, ListFormCell>();
    }
}
