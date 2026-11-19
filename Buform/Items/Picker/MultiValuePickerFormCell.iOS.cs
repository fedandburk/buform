using ObjCRuntime;

namespace Buform;

public abstract class MultiValuePickerFormCell<TItem> : PresentedPickerFormCell<TItem>
    where TItem : class, IMultiValuePickerFormItem
{
    protected MultiValuePickerFormCell()
    {
        /* Required constructor */
    }

    protected MultiValuePickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}

[Register(nameof(MultiValuePickerFormCell))]
public sealed class MultiValuePickerFormCell : MultiValuePickerFormCell<IMultiValuePickerFormItem>
{
    public MultiValuePickerFormCell()
    {
        /* Required constructor */
    }

    public MultiValuePickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
