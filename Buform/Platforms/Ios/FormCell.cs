using System.ComponentModel;
using Fedandburk.iOS.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class FormCell : UITableViewCell
{
    private bool _isInitialized;

    public virtual bool IsSelectable => false;

    public IFormItem? Item { get; private set; }

    protected FormCell()
    {
        /* Required constructor */
    }

    protected FormCell(IntPtr handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected FormCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
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

    protected static UIViewController? GetViewController()
    {
        return UIApplication.SharedApplication.GetTopViewController();
    }

    protected abstract void Initialize();

    public virtual void Initialize(IFormItem item)
    {
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

    protected FormCell()
    {
        /* Required constructor */
    }

    protected FormCell(IntPtr handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected FormCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
    {
        /* Required constructor */
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Item == null)
        {
            return;
        }

        OnItemPropertyChanged(e.PropertyName);
    }

    protected abstract void OnItemSet();

    protected abstract void OnItemPropertyChanged(string? propertyName);

    public override void Initialize(IFormItem item)
    {
        base.Initialize(item);

        if (ReferenceEquals(Item, item))
        {
            return;
        }

        if (Item != null)
        {
            Item.PropertyChanged -= OnItemPropertyChanged;
        }

        Item = item as TItem;

        if (Item == null)
        {
            return;
        }

        Item.PropertyChanged += OnItemPropertyChanged;

        OnItemSet();
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
