using MvvmCross.Binding.BindingContext;

namespace Buform;

public abstract class MvxFormCell<TItem> : FormCell<TItem>, IMvxBindingContextOwner
    where TItem : class, IFormItem
{
    public IMvxBindingContext? BindingContext { get; set; }

    protected MvxFormCell()
    {
        /* Required constructor */
    }

    protected MvxFormCell(IntPtr handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected MvxFormCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
    {
        /* Required constructor */
    }

    protected override void OnItemSet()
    {
        BindingContext = new MvxBindingContext(Item);

        InitializeBindings();
    }

    protected MvxFluentBindingDescriptionSet<MvxFormCell<TItem>, TItem> CreateBindingSet()
    {
        return new MvxFluentBindingDescriptionSet<MvxFormCell<TItem>, TItem>(this);
    }

    protected abstract void InitializeBindings();

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        /* Nothing to do */
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            BindingContext?.ClearAllBindings();
        }
    }
}
