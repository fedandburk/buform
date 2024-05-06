using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(StepperFormCell))]
public sealed class StepperFormCell : StepperFormCellBase<StepperFormItem>
{
    public StepperFormCell()
    {
        /* Required constructor */
    }

    public StepperFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
