using System.ComponentModel;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class FormViewHolder : RecyclerView.ViewHolder
{
    private bool _isInitialized;

    public IDisposable? Data { get; private set; }

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

    public virtual void Initialize(IDisposable data)
    {
        if (ReferenceEquals(Data, data))
        {
            return;
        }

        Data = data;

        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        Initialize();
    }
}

[Preserve(AllMembers = true)]
public abstract class FormViewHolder<TData> : FormViewHolder
    where TData : class, INotifyPropertyChanged, IDisposable
{
    public new TData? Data { get; private set; }

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

    private void OnDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Data == null)
        {
            return;
        }

        OnDataPropertyChanged(e.PropertyName);
    }

    protected abstract void OnDataSet();

    protected abstract void OnDataPropertyChanged(string? propertyName);

    public override void Initialize(IDisposable data)
    {
        base.Initialize(data);

        if (ReferenceEquals(Data, data))
        {
            return;
        }

        if (Data != null)
        {
            Data.PropertyChanged -= OnDataPropertyChanged;
        }

        Data = data as TData;

        if (Data == null)
        {
            return;
        }

        Data.PropertyChanged += OnDataPropertyChanged;

        OnDataSet();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var data = Data;
            if (data != null)
            {
                data.PropertyChanged -= OnDataPropertyChanged;
            }

            Data = null;
        }

        base.Dispose(disposing);
    }
}
