using Android.Runtime;
using Android.Views;

namespace Buform;

public class LabeledFormGroupHeaderViewHolder : FormViewHolder<ILabeledFormGroup>
{
    protected virtual TextView? TextView { get; set; }

    public LabeledFormGroupHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public LabeledFormGroupHeaderViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        TextView = ItemView.FindViewById<TextView>(Resource.Id.TextView);
    }

    protected virtual void UpdateLabel()
    {
        var label = Data?.HeaderLabel;

        var isVisible = !string.IsNullOrEmpty(label);

        if (TextView == null)
        {
            return;
        }

        TextView.Text = label;
        TextView.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
    }

    protected override void OnDataSet()
    {
        UpdateLabel();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ILabeledFormGroup.HeaderLabel):
            case null:
                UpdateLabel();
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
