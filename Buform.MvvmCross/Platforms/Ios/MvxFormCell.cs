using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace Buform;

public abstract class MvxFormCell<TItem> : FormCell<TItem>, IMvxBindable
    where TItem : class, IFormItem
{
    public IMvxBindingContext BindingContext { get; set; } = null!;

    public object? DataContext
    {
        get => BindingContext.DataContext;
        set => BindingContext.DataContext = value;
    }

    protected MvxFormCell()
    {
        this.CreateBindingContext(string.Empty);
    }

    protected MvxFormCell(IntPtr handle)
        : base(handle)
    {
        this.CreateBindingContext(string.Empty);
    }

    protected MvxFormCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
    {
        this.CreateBindingContext(string.Empty);
    }

    protected override void OnItemSet()
    {
        DataContext = Item;

        this.DelayBind(InitializeBindings);
    }

    protected MvxFluentBindingDescriptionSet<MvxFormCell<TItem>, TItem> CreateBindingSet()
    {
        return this.CreateBindingSet<MvxFormCell<TItem>, TItem>();
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
            BindingContext.ClearAllBindings();
        }
    }
}
