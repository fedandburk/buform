using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class TextFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : class, ITextFormItem
{
    protected TextFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected TextFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected virtual TextView? LabelView { get; set; }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Text);
    }

    protected virtual void UpdateReadOnlyState()
    {
        LabelView?.Enabled = !(Item?.IsReadOnly ?? true);
    }

    protected virtual void UpdateValue()
    {
        LabelView?.Text = Item?.FormattedValue ?? Item?.Value?.ToString();
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ITextFormItem.Value):
            case nameof(ITextFormItem.FormattedValue):
                UpdateValue();
                break;
            case null:
                UpdateReadOnlyState();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LabelView = null;
        }

        base.Dispose(disposing);
    }
}

public sealed class TextFormViewHolder : TextFormViewHolder<ITextFormItem>
{
    public TextFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public TextFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
