// ReSharper disable UnusedType.Global

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
public sealed class PickerFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItemClass<IAsyncPickerFormItem, AsyncPickerFormCell>();
        FormPlatform.RegisterItemClass<ICallbackPickerFormItem, CallbackPickerFormCell>();
        FormPlatform.RegisterItemClass<IMultiValuePickerFormItem, MultiValuePickerFormCell>();
        FormPlatform.RegisterItemClass<IPickerFormItem, PopUpPickerFormCell>();
    }

    public static class Texts
    {
        public static string Clear = "Clear";
        public static string Cancel = "Cancel";
    }
}
