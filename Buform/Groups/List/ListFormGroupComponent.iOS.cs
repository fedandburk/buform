// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class ListFormGroupComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterGroupHeaderClass<IListFormGroup, ListFormGroupHeader>();
        FormPlatform.RegisterGroupFooterClass<IListFormGroup, ListFormGroupFooter>();
        FormPlatform.RegisterGroupHandler<IListFormGroup, ListFormGroupHandler>();
    }
}
