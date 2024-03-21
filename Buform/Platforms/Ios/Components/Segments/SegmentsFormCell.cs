using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(SegmentsFormCell))]
public sealed class SegmentsFormCell : SegmentsFormCellBase<ISegmentsFormItem>
{
    public SegmentsFormCell()
    {
        /* Required constructor */
    }

    public SegmentsFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}