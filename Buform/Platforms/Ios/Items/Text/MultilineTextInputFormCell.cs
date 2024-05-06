using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(MultilineTextInputFormCell))]
public sealed class MultilineTextInputFormCell
    : MultilineTextInputFormCellBase<IMultilineTextInputFormItem>
{
    public MultilineTextInputFormCell()
    {
        /* Required constructor */
    }

    public MultilineTextInputFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
