using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class TextFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<IMultilineTextInputFormItem, TextMultilineFormViewHolder>(
            Resource.Layout.FormItemTextMultilineLayout
        );

        FormPlatform.RegisterItem<ITextInputFormItem, TextInputFormViewHolder>(
            Resource.Layout.FormItemTextInputLayout
        );

        FormPlatform.RegisterItem<ITextFormItem, TextFormViewHolder>(
            Resource.Layout.FormItemTextLayout
        );
    }
}
