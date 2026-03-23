using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public class SegmentsFormItemComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterItem<ISegmentsFormItem, SegmentsFormViewHolder>(
            Resource.Layout.FormItemSegmentsLayout
        );
    }
}
