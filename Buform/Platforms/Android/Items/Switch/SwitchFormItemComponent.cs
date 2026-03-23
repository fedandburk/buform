using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public class SwitchFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<SwitchFormItem, SwitchFormViewHolder>(
            Resource.Layout.FormItemSwitchLayout
        );
    }
}
