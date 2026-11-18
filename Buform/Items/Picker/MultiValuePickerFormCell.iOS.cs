using ObjCRuntime;

namespace Buform;

[Register(nameof(MultiValuePickerFormCell))]
public sealed class MultiValuePickerFormCell
    : PresentedPickerFormCellBase<IMultiValuePickerFormItem>
{
    public MultiValuePickerFormCell()
    {
        /* Required constructor */
    }

    public MultiValuePickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
