using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class LogoFormGroupFooterViewHolder : FormViewHolder<LogoFormGroup>
{
    public LogoFormGroupFooterViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public LogoFormGroupFooterViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        /* Nothing to do */
    }

    protected override void OnDataSet()
    {
        /* Nothing to do */
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        /* Nothing to do */
    }
}
