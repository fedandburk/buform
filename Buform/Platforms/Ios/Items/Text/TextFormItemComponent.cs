namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class TextFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<IMultilineTextInputFormItem, MultilineTextInputFormCell>();
        FormPlatform.RegisterItemClass<ITextInputFormItem, TextInputFormCell>();
        FormPlatform.RegisterItemClass<ITextFormItem, TextFormCell>();
    }
}
