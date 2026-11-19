using Android.Views;

namespace Buform.Example.Droid;

public sealed class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
{
    private readonly Action _onClick;

    public MenuItemClickListener(Action onClick)
    {
        ArgumentNullException.ThrowIfNull(onClick);

        _onClick = onClick;
    }

    public bool OnMenuItemClick(IMenuItem item)
    {
        _onClick();

        return true;
    }
}
