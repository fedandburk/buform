namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class ListFormGroupComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterGroupHeaderClass<IListFormGroup, ListFormGroupHeader>();
        FormPlatform.RegisterGroupFooterClass<IListFormGroup, ListFormGroupFooter>();
        FormPlatform.RegisterGroupHandler<IListFormGroup, ListFormGroupHandler>();
    }
}
