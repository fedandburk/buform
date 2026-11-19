using Android.Runtime;
using Android.Views;

namespace Buform.Example.Droid;

public sealed class LogoFormGroupHeaderViewHolder : FormHeaderFooterViewHolder<LogoFormGroup>
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
}
