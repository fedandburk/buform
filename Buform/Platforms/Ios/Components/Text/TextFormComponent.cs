namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class TextFormComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterGroupHeaderClass<TextFormGroup, TextFormGroupHeader>();
        FormPlatform.RegisterGroupFooterClass<TextFormGroup, TextFormGroupFooter>();

        FormPlatform.RegisterItemClass<IMultilineTextFormItem, MultilineTextFormCell>();
        FormPlatform.RegisterItemClass<ITextFormItem, TextFormCell>();
    }
}
