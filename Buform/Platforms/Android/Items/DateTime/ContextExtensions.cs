using Android.Content;
using AndroidX.Fragment.App;

namespace Buform;

internal static class ContextExtensions
{
    public static FragmentActivity? GetFragmentActivity(this Context context)
    {
        var current = context;

        while (true)
        {
            if (current is FragmentActivity fragmentActivity)
            {
                return fragmentActivity;
            }

            if (current is ContextWrapper wrapper)
            {
                current = wrapper.BaseContext!;
                continue;
            }

            break;
        }

        return null;
    }
}
