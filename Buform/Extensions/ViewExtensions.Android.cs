using Android.Content;
using Android.Views;
using AndroidX.Fragment.App;

namespace Buform;

internal static class ViewExtensions
{
    public static FragmentActivity GetFragmentActivity(this View view)
    {
        ArgumentNullException.ThrowIfNull(view);

        var context = view.Context;

        while (context is ContextWrapper wrapper)
        {
            if (context is FragmentActivity activity)
            {
                return activity;
            }

            context = wrapper.BaseContext;
        }

        throw new InvalidOperationException(
            "The form view is not hosted in an AndroidX FragmentActivity."
        );
    }
}
