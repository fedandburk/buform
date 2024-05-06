using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
// ReSharper disable once UnusedType.Global
public sealed class TextFormGroupComponent : IFormComponent
{
    public void Register()
    {
        FormPlatform.RegisterGroupHeader<TextFormGroup, TextFormGroupHeaderViewHolder>(
            Resource.Layout.FormGroupTextHeaderLayout
        );

        FormPlatform.RegisterGroupFooter<TextFormGroup, TextFormGroupFooterViewHolder>(
            Resource.Layout.FormGroupTextFooterLayout
        );
    }
}
