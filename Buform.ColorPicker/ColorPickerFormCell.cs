using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(ColorPickerFormCell))]
public sealed class ColorPickerFormCell : ColorPickerFormCellBase<ColorPickerFormItem>
{
    public ColorPickerFormCell()
    {
        /* Required constructor */
    }

    public ColorPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
