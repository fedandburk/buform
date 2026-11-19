using Android.Views;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;

namespace Buform;

public class PopUpPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    private const int OptionsGroupId = 0;

    private PopupMenu? _menu;

    public override void Pick(View anchorView, TItem item)
    {
        ArgumentNullException.ThrowIfNull(anchorView);
        ArgumentNullException.ThrowIfNull(item);

        var context = anchorView.Context
            ?? throw new InvalidOperationException("The anchor view has no context.");

        var options = item.Options.ToList();

        _menu?.Dismiss();
        _menu = new PopupMenu(context, anchorView, (int)GravityFlags.End);

        for (var index = 0; index < options.Count; index++)
        {
            var option = options[index];
            var menuItem = _menu.Menu.Add(OptionsGroupId, index, index, option.FormattedValue);

            menuItem?.SetChecked(item.IsPicked(option));
        }

        _menu.Menu.SetGroupCheckable(OptionsGroupId, true, true);

        _menu.MenuItemClick += (_, args) =>
        {
            var menuItem = args.Item
                ?? throw new InvalidOperationException("The menu item is missing.");

            item.Pick(options[menuItem.ItemId]);

            args.Handled = true;
        };

        _menu.Show();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _menu?.Dismiss();
            _menu?.Dispose();
            _menu = null;
        }

        base.Dispose(disposing);
    }
}
