using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(AsyncPickerFormCell))]
public sealed class AsyncPickerFormCell : AsyncPickerFormCellBase<IAsyncPickerFormItem>
{
    public AsyncPickerFormCell()
    {
        /* Required constructor */
    }

    public AsyncPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
