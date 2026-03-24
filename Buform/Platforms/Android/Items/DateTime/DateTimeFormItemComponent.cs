using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public class DateTimeFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<DateTimeFormItem, DateTimeFormViewHolder>(
            Resource.Layout.FormItemDateTimeLayout
        );
    }
}
