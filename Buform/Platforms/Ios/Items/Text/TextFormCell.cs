using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(TextFormCell))]
public sealed class TextFormCell : TextFormCellBase<ITextFormItem>
{
    public TextFormCell()
    {
        /* Required constructor */
    }

    public TextFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
