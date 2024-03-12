using Buform.Example.Core;
using ObjCRuntime;

namespace Buform.Example.MvvmCross.iOS;

[Preserve(AllMembers = true)]
[Register(nameof(HeaderFormGroupHeader))]
public sealed class HeaderFormGroupHeader : FormHeaderFooter<HeaderFormGroup>
{
    public HeaderFormGroupHeader()
    {
        /* Required constructor */
    }

    public HeaderFormGroupHeader(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
