using System.ComponentModel;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

public abstract class FormViewHolder : RecyclerView.ViewHolder
{
    private bool _isInitialized;

    public IFormItem? FormItem { get; private set; }

    protected FormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected FormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected abstract void Initialize();

    public virtual void Initialize(IFormItem item)
    {
        if (ReferenceEquals(FormItem, item))
        {
            return;
        }

        FormItem = item;

        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        Initialize();
    }
}

public abstract class FormViewHolder<TItem> : FormViewHolder
    where TItem : class, IFormItem
{
    protected TItem? Item { get; private set; }

    protected FormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected FormViewHolder(View itemView)
        : base(itemView)
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
