using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(DateTimeFormCell))]
public sealed class DateTimeFormCell : DateTimeFormCellBase<DateTimeFormItem>
{
    public DateTimeFormCell()
    {
        /* Required constructor */
    }

    public DateTimeFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
