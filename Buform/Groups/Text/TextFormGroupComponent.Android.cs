// ReSharper disable UnusedType.Global

using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
[FormComponent]
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
