using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public class PickerFormItemComponent : IFormComponent
{
    public void Register()
    {
        // FormPlatform.RegisterItem<IAsyncPickerFormItem, AsyncPickerFormViewHolder>(
        //     Resource.Layout.FormItemDateTimeLayout
        // );

        // FormPlatform.RegisterItem<ICallbackPickerFormItem, CallbackPickerFormViewHolder>(
        //     Resource.Layout.FormItemDateTimeLayout
        // );

        // FormPlatform.RegisterItem<IMultiValuePickerFormItem, MultiValuePickerFormViewHolder>(
        //     Resource.Layout.FormItemDateTimeLayout
        // );


        FormPlatform.RegisterItem<IPickerFormItem, PopUpPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
    }
}
