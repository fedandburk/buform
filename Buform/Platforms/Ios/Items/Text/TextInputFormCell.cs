using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(TextInputFormCell))]
public sealed class TextInputFormCell : TextInputFormCellBase<ITextInputFormItem>
{
    public TextInputFormCell()
    {
        /* Required constructor */
    }

    public TextInputFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
