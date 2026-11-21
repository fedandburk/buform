using Android.Runtime;
using Android.Views;

namespace Buform;

public class LabeledFormGroupHeaderViewHolder : FormHeaderFooterViewHolder<ILabeledFormGroup>
{
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

    protected virtual TextView? LabelView { get; set; }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
    }

    protected virtual void UpdateLabel()
    {
        var label = Group?.HeaderLabel;

        var isVisible = !string.IsNullOrEmpty(label);

        if (LabelView == null)
        {
            return;
        }

        LabelView.Text = label;
        LabelView.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
    }

    protected override void OnGroupSet()
    {
        UpdateLabel();
    }

    protected override void OnGroupPropertyChanged(string? propertyName)
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
            LabelView = null;
        }

        base.Dispose(disposing);
    }
}
