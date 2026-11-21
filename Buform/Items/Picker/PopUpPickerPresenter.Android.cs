using Android.Views;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;

namespace Buform;

public class PopUpPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    private const int OptionsGroupId = 0;

    private PopupMenu? _menu;

    public override Task PickAsync(View sourceView, TItem item)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        ArgumentNullException.ThrowIfNull(item);

        var context = sourceView.Context
            ?? throw new InvalidOperationException("The source view has no context.");

        var options = item.Options.ToList();

        _menu?.Dismiss();
        _menu = new PopupMenu(context, sourceView, (int)GravityFlags.End);

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

        return Task.CompletedTask;
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
