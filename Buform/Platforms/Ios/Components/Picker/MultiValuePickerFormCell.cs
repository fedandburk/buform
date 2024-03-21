using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(MultiValuePickerFormCell))]
public sealed class MultiValuePickerFormCell : MultiValuePickerFormCellBase<IMultiValuePickerFormItem>
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