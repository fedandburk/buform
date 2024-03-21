using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(PopUpPickerFormCell))]
public sealed class PopUpPickerFormCell : PopUpPickerFormCellBase<IPickerFormItem> 
{
    public PopUpPickerFormCell()
    {
        /* Required constructor */
    }

    public PopUpPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}