using System.ComponentModel;
using ObjCRuntime;

namespace Buform;

public abstract class FormCell : UITableViewCell
{
    private bool _isInitialized;

    public virtual bool IsSelectable => false;

    public IFormItem? Item { get; private set; }

    protected FormCell()
    {
        /* Required constructor */
    }

    protected FormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected UITableView? GetTableView()
    {
        var view = this as UIView;

        while (true)
        {
            switch (view.Superview)
            {
                case null:
                    return null;
                case UITableView tableView:
                    return tableView;
                default:
                    view = view.Superview;
                    break;
            }
        }
    }

    protected UIViewController? GetViewController()
    {
        return this.FindViewController();
    }

    protected abstract void Initialize();

    public virtual void Initialize(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (ReferenceEquals(Item, item))
        {
            return;
        }

        Item = item;

        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        Initialize();
    }

    public virtual void OnSelected()
    {
        /* Nothing to do */
    }
}

public abstract class FormCell<TItem> : FormCell
    where TItem : class, IFormItem
{
    public new TItem? Item { get; private set; }

    protected virtual UILabel? ValidationLabel => null;

    protected FormCell()
    {
        /* Required constructor */
    }

    protected FormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Item == null)
        {
            return;
        }

        if (e.PropertyName == nameof(IValidatableFormItem.ValidationErrorMessage))
        {
            UpdateValidationLabel();
        }

        OnItemPropertyChanged(e.PropertyName);
    }

    private void UpdateValidationLabel()
    {
        if (ValidationLabel == null || Item is not IValidatableFormItem item)
        {
            return;
        }

        ValidationLabel.TextColor =
            item.ValidationErrorMessage == null ? UIColor.Label : UIColor.SystemRed;
    }

    protected virtual void OnItemSet()
    {
        /* Nothing to do */
    }

    protected virtual void OnItemPropertyChanged(string? propertyName)
    {
        /* Nothing to do */
    }

    public override void Initialize(IFormItem item)
    {
        base.Initialize(item);

        if (ReferenceEquals(Item, item))
        {
            return;
        }

        if (item is not TItem typedItem)
        {
            throw new ArgumentException(
                $"The item must be of type '{typeof(TItem).FullName}'.",
                nameof(item)
            );
        }

        if (Item != null)
        {
            Item.PropertyChanged -= OnItemPropertyChanged;
        }

        Item = typedItem;

        Item.PropertyChanged += OnItemPropertyChanged;

        OnItemSet();

        UpdateValidationLabel();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var item = Item;
            if (item != null)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }

            Item = null;
        }

        base.Dispose(disposing);
    }
}
