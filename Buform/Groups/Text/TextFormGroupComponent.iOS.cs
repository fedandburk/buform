// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class TextFormGroupComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterGroupHeaderClass<TextFormGroup, TextFormGroupHeader>();
        FormPlatform.RegisterGroupFooterClass<TextFormGroup, TextFormGroupFooter>();
    }
}
