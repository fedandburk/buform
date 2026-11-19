using Android.Runtime;
using Android.Views;

namespace Buform;

public class TextFormViewHolder : FormViewHolder<ITextFormItem>
{
    protected virtual TextView? TextView { get; set; }

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

    protected override void Initialize()
    {
        TextView = ItemView.FindViewById<TextView>(Resource.Id.Text);
    }

    protected virtual void UpdateReadOnlyState()
    {
        TextView?.Enabled = !(Data?.IsReadOnly ?? true);
    }

    protected virtual void UpdateText()
    {
        TextView?.Text = Data?.FormattedValue ?? Data?.Value?.ToString();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateText();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ITextFormItem.Value):
            case nameof(ITextFormItem.FormattedValue):
                UpdateText();
                break;
            case null:
                OnDataSet();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TextView = null;
        }

        base.Dispose(disposing);
    }
}
