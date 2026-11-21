using System.ComponentModel;
using ObjCRuntime;

namespace Buform;

public abstract class FormHeaderFooter : UITableViewHeaderFooterView
{
    protected FormHeaderFooter()
    {
        /* Required constructor */
    }

    protected FormHeaderFooter(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public abstract void Initialize(IFormGroup group);

    protected UITableView? GetTableView()
    {
        UIView view = this;

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
}

public abstract class FormHeaderFooter<TGroup> : FormHeaderFooter
    where TGroup : class, IFormGroup
{
    protected FormHeaderFooter()
    {
        /* Required constructor */
    }

    protected FormHeaderFooter(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    public TGroup? Group { get; private set; }

    public sealed override void Initialize(IFormGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (ReferenceEquals(Group, group))
        {
            return;
        }

        if (group is not TGroup typedGroup)
        {
            throw new ArgumentException(
                $"The group must be of type '{typeof(TGroup).FullName}'.",
                nameof(group)
            );
        }

        Group?.PropertyChanged -= OnGroupPropertyChanged;

        Group = typedGroup;

        Group.PropertyChanged += OnGroupPropertyChanged;

        OnGroupSet();
    }

    protected virtual void OnGroupSet()
    {
        /* Nothing to do */
    }

    protected virtual void OnGroupPropertyChanged(string? propertyName)
    {
        /* Nothing to do */
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var group = Group;
            group?.PropertyChanged -= OnGroupPropertyChanged;

            Group = null;
        }

        base.Dispose(disposing);
    }

    private void OnGroupPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Group == null)
        {
            return;
        }

        OnGroupPropertyChanged(e.PropertyName);
    }
}
