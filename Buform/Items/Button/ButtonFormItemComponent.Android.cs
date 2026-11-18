// ReSharper disable UnusedType.Global

using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class ButtonFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<ButtonFormItem, ButtonFormViewHolder>(
            Resource.Layout.FormItemButtonLayout
        );
    }
}
