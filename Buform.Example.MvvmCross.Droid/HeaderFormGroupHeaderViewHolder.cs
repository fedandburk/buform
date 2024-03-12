using Android.Runtime;
using Android.Views;
using Buform.Example.Core;

namespace Buform.Example.MvvmCross.Droid;

[Preserve(AllMembers = true)]
public sealed class HeaderFormGroupHeaderViewHolder : FormViewHolder<HeaderFormGroup>
{
    public HeaderFormGroupHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public HeaderFormGroupHeaderViewHolder(View itemView)
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
