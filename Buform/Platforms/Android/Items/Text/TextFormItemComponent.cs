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
            Resource.Layout.FormItemTexMultilineLayout
        );
        
        FormPlatform.RegisterItem<ITextInputFormItem, TextInputFormViewHolder>(
            Resource.Layout.FormItemTexInputLayout
        );

        FormPlatform.RegisterItem<ITextFormItem, TextFormViewHolder>(
            Resource.Layout.FormItemTextLayout
        );
    }
}
