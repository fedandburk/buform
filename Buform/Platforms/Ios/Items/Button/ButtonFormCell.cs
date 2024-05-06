using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(ButtonFormCell))]
public sealed class ButtonFormCell : ButtonFormCellBase<ButtonFormItem>
{
    public ButtonFormCell(NSString reuseIdentifier)
        : base(reuseIdentifier)
    {
        /* Required constructor */
    }

    public ButtonFormCell()
    {
        /* Required constructor */
    }

    public ButtonFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
