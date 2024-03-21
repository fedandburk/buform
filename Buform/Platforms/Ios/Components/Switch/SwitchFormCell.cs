using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(SwitchFormCell))]
public sealed class SwitchFormCell : SwitchFormCellBase<SwitchFormItem>
{
    public SwitchFormCell()
    {
        /* Required constructor */
    }

    public SwitchFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}