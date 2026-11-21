using System.ComponentModel;
using Android.Runtime;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;

namespace Buform;

public abstract class FormViewHolder : RecyclerView.ViewHolder
{
    private bool _isInitialized;
    private IDisposable? _data;

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

    public virtual void Initialize(IDisposable data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (ReferenceEquals(_data, data))
        {
            return;
        }

        _data = data;

        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        Initialize();
    }

    protected FragmentActivity GetFragmentActivity()
    {
        return ItemView.GetFragmentActivity();
    }

    protected abstract void Initialize();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _data = null;
        }

        base.Dispose(disposing);
    }
}

public abstract class FormViewHolder<TItem> : FormViewHolder
    where TItem : class, IFormItem
{
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

    public TItem? Item { get; private set; }

    protected virtual TextView? ValidationErrorView { get; set; }

    public override void Initialize(IDisposable data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data is not TItem item)
        {
            throw new ArgumentException(
                $"The data must be of type '{typeof(TItem).FullName}', but it is of type '{data.GetType().FullName}'.",
                nameof(data)
            );
        }

        base.Initialize(item);

        if (ReferenceEquals(Item, item))
        {
            return;
        }

        Item?.PropertyChanged -= OnItemPropertyChanged;

        Item = item;
        Item.PropertyChanged += OnItemPropertyChanged;

        UpdateValidationErrorView();

        OnItemSet();
    }

    protected virtual void OnItemSet()
    {
        /* Nothing to do */
    }

    protected virtual void OnItemPropertyChanged(string? propertyName)
    {
        /* Nothing to do */
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Item?.PropertyChanged -= OnItemPropertyChanged;
            Item = null;

            ValidationErrorView = null;
        }

        base.Dispose(disposing);
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IValidatableFormItem.ValidationErrorMessage) or null)
        {
            UpdateValidationErrorView();
        }

        OnItemPropertyChanged(e.PropertyName);
    }

    private void UpdateValidationErrorView()
    {
        if (ValidationErrorView == null || Item is not IValidatableFormItem item)
        {
            return;
        }

        var message = item.ValidationErrorMessage;

        ValidationErrorView.Text = message;
        ValidationErrorView.Visibility = message == null ? ViewStates.Gone : ViewStates.Visible;
    }
}
