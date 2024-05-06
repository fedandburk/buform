namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class TextFormGroupComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterGroupHeaderClass<TextFormGroup, TextFormGroupHeader>();
        FormPlatform.RegisterGroupFooterClass<TextFormGroup, TextFormGroupFooter>();
    }
}
