using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class ButtonFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<ButtonFormItem, ButtonFormViewHolder>(
            Resource.Layout.FormItemButtonLayout
        );
    }
}
