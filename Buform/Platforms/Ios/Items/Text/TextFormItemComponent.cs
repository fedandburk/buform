namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class TextFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<IMultilineTextFormItem, MultilineTextFormCell>();
        FormPlatform.RegisterItemClass<ITextFormItem, TextFormCell>();
    }
}
