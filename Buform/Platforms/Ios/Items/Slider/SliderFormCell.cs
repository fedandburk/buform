using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(SliderFormCell))]
public sealed class SliderFormCell : SliderFormCellBase<SliderFormItem>
{
    public SliderFormCell()
    {
        /* Required constructor */
    }

    public SliderFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
