using Android.Runtime;
using Android.Views;

namespace Buform.Example.Droid;

public sealed class LogoFormGroupFooterViewHolder : FormHeaderFooterViewHolder<LogoFormGroup>
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
}
