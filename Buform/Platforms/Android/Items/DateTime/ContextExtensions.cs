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
                var baseContext = wrapper.BaseContext;
                if (baseContext is null || ReferenceEquals(baseContext, current))
                {
                    break;
                }

                current = baseContext;
                continue;
            }

            break;
        }

        return null;
    }
}
