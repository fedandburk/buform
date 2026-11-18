using ObjCRuntime;

namespace Buform;

[Register(nameof(LogoFormHeader))]
public sealed class LogoFormHeader : FormHeaderFooter<LogoFormGroup>
{
    public LogoFormHeader()
    {
        /* Required constructor */
    }

    public LogoFormHeader(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
