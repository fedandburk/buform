using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(CallbackPickerFormCell))]
public sealed class CallbackPickerFormCell : CallbackPickerFormCellBase<ICallbackPickerFormItem>
{
    public CallbackPickerFormCell()
    {
        /* Required constructor */
    }

    public CallbackPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
