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
            switch (current)
            {
                case FragmentActivity fragmentActivity:
                    return fragmentActivity;
                case ContextWrapper wrapper:
                    current = wrapper.BaseContext!;
                    continue;
            }

            break;
        }

        return null;
    }
}
