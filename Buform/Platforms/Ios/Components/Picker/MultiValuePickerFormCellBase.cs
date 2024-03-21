using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class MultiValuePickerFormCellBase<TMultiValuePickerItem> : PresentedPickerFormCellBase<TMultiValuePickerItem> where TMultiValuePickerItem : class, IMultiValuePickerFormItem
{
    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    protected MultiValuePickerFormCellBase()
    {
        /* Required constructor */
    }

    protected MultiValuePickerFormCellBase(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
