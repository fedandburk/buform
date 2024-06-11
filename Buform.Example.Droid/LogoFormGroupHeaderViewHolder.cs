using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class LogoFormGroupHeaderViewHolder : FormViewHolder<LogoFormGroup>
{
    public LogoFormGroupHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public LogoFormGroupHeaderViewHolder(View itemView)
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
