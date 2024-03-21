using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(MultilineTextFormCell))]
public sealed class MultilineTextFormCell : MultilineTextFormCellBase<IMultilineTextFormItem>
{
    public MultilineTextFormCell()
    {
        /* Required constructor */
    }

    public MultilineTextFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}