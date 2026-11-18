// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class TextFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<IMultilineTextInputFormItem, MultilineTextInputFormCell>();
        FormPlatform.RegisterItemClass<ITextInputFormItem, TextInputFormCell>();
        FormPlatform.RegisterItemClass<ITextFormItem, TextFormCell>();
    }
}
