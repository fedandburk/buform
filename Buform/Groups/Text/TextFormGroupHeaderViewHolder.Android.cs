using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class TextFormGroupHeaderViewHolder : FormViewHolder<TextFormGroup>
{
    private TextView? _textView;

    public TextFormGroupHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public TextFormGroupHeaderViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        _textView = ItemView.FindViewById<TextView>(Resource.Id.TextView)!;
    }

    private void UpdateLabel()
    {
        if (_textView == null)
        {
            return;
        }

        _textView.Text = Data?.HeaderLabel;
    }

    protected override void OnDataSet()
    {
        UpdateLabel();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Data.HeaderLabel):
                UpdateLabel();
                break;
        }
    }
}
