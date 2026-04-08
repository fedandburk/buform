using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class DateTimeFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<DateTimeFormItem, DateTimeFormViewHolder>(
            Resource.Layout.FormItemDateTimeLayout
        );
    }
}
