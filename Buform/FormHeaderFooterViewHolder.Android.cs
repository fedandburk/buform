using System.ComponentModel;
using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class FormHeaderFooterViewHolder<TGroup> : FormViewHolder
    where TGroup : class, IFormGroup
{
    public TGroup? Group { get; private set; }

    protected FormHeaderFooterViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected FormHeaderFooterViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnGroupPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnGroupPropertyChanged(e.PropertyName);
    }

    protected virtual void OnGroupSet()
    {
        /* Nothing to do */
    }

    protected virtual void OnGroupPropertyChanged(string? propertyName)
    {
        /* Nothing to do */
    }

    public override void Initialize(IDisposable data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data is not TGroup group)
        {
            throw new ArgumentException(
                $"The data must be of type '{typeof(TGroup).FullName}', but it is of type '{data.GetType().FullName}'.",
                nameof(data)
            );
        }

        base.Initialize(group);

        if (ReferenceEquals(Group, group))
        {
            return;
        }

        Group?.PropertyChanged -= OnGroupPropertyChanged;

        Group = group;
        Group.PropertyChanged += OnGroupPropertyChanged;

        OnGroupSet();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Group?.PropertyChanged -= OnGroupPropertyChanged;
            Group = null;
        }

        base.Dispose(disposing);
    }
}
