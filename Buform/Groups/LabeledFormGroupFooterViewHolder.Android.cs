using Android.Runtime;
using Android.Views;

namespace Buform;

public class LabeledFormGroupFooterViewHolder : FormViewHolder<ILabeledFormGroup>
{
    protected virtual TextView? TextView { get; set; }

    public LabeledFormGroupFooterViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public LabeledFormGroupFooterViewHolder(View itemView)
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
        var label = Data?.FooterLabel;

        var isVisible = !string.IsNullOrEmpty(label);

        TextView?.Text = label;

        var layoutParameters = ItemView.LayoutParameters
            ?? throw new InvalidOperationException("The view has no layout parameters.");

        layoutParameters.Height = isVisible ? ViewGroup.LayoutParams.WrapContent : 0;

        ItemView.LayoutParameters = layoutParameters;
        ItemView.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
    }

    protected override void OnDataSet()
    {
        UpdateLabel();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ILabeledFormGroup.FooterLabel):
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
