using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class PickerFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<IPickerFormItem, PopUpPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );

        FormPlatform.RegisterItem<IMultiValuePickerFormItem, MultiValuePickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );

        FormPlatform.RegisterItem<ICallbackPickerFormItem, CallbackPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );

        FormPlatform.RegisterItem<IAsyncPickerFormItem, AsyncPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
    }
}
