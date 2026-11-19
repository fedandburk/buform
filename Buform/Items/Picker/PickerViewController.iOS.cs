// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract

using System.ComponentModel;

namespace Buform;

public class PickerViewController<TItem> : UITableViewController
    where TItem : class, IOptionsPickerFormItem
{
    private readonly CGSize _minimumPopUpSize = new(240, 320);

    protected TItem? Item { get; private set; }

    public override CGSize PreferredContentSize
    {
        get => base.PreferredContentSize;
        set
        {
            base.PreferredContentSize = value;

            if (NavigationController?.PopoverPresentationController == null)
            {
                return;
            }

            NavigationController.PreferredContentSize = PreferredContentSize;
        }
    }

    public PickerViewController(UITableViewStyle style, TItem item)
        : base(style)
    {
        ArgumentNullException.ThrowIfNull(item);

        Item = item;

        Item.PropertyChanged += OnPropertyChanged;
    }

    protected virtual void SetLeftBarButtonCloseItem()
    {
        NavigationItem.LeftBarButtonItem = new UIBarButtonItem(
            UIBarButtonSystemItem.Close,
            (_, _) => DismissViewController(true, null)
        );
    }

    protected virtual void SetRightBarButtonClearItem()
    {
        NavigationItem.RightBarButtonItem = new UIBarButtonItem(
            FormTexts.Clear,
            UIBarButtonItemStyle.Plain,
            (_, _) =>
            {
                if (Item != null)
                {
                    Item.Value = null;
                }
            }
        );
    }

    protected virtual async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);

        InvokeOnMainThread(() => TableView.ReloadData());
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = Item?.Label;

        if (NavigationController?.PopoverPresentationController != null)
        {
            SetLeftBarButtonCloseItem();
        }

        if (Item?.CanBeCleared ?? false)
        {
            SetRightBarButtonClearItem();
        }

        NavigationItem.SearchController = new UISearchController()
        {
            HidesNavigationBarDuringPresentation = false,
        };

        NavigationItem.SearchController.SearchBar.TextChanged += OnSearchTextChanged;
        NavigationItem.SearchController.SearchBar.CancelButtonClicked +=
            OnSearchCancelButtonClicked;
    }

    private void OnSearchTextChanged(object? sender, UISearchBarTextChangedEventArgs e)
    {
        if (Item is null)
        {
            return;
        }

        Item.FilterQuery = e.SearchText;
    }

    private void OnSearchCancelButtonClicked(object? sender, EventArgs e)
    {
        if (Item is null)
        {
            return;
        }

        Item.FilterQuery = null;
    }

    public override nint RowsInSection(UITableView tableView, nint section)
    {
        return Item?.Options.Count() ?? 0;
    }

    public override nint NumberOfSections(UITableView tableView)
    {
        return 1;
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        return TableView.DequeueReusableCell(nameof(UITableViewCell))
            ?? new UITableViewCell(UITableViewCellStyle.Default, nameof(UITableViewCell));
    }

    public override void WillDisplay(
        UITableView tableView,
        UITableViewCell cell,
        NSIndexPath indexPath
    )
    {
        var item = Item?.Options.ElementAt(indexPath.Row);

        if (item == null)
        {
            return;
        }

        if (Item == null)
        {
            return;
        }

        var configuration = cell.DefaultContentConfiguration;
        configuration.Text = item.FormattedValue;

        cell.ContentConfiguration = configuration;

        cell.Accessory = Item.IsPicked(item)
            ? UITableViewCellAccessory.Checkmark
            : UITableViewCellAccessory.None;
    }

    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        var item = Item?.Options.ElementAt(indexPath.Row);

        if (item == null)
        {
            return;
        }

        Item?.Pick(item);
    }

    public override string TitleForFooter(UITableView tableView, nint section)
    {
        return Item?.Message ?? string.Empty;
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();

        if (NavigationController?.PopoverPresentationController != null)
        {
            PreferredContentSize =
                TableView.ContentSize.Height < _minimumPopUpSize.Height
                    ? _minimumPopUpSize
                    : TableView.ContentSize;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var item = Item;
            if (item != null)
            {
                item.PropertyChanged -= OnPropertyChanged;
            }

            Item = null;

            NavigationItem.SearchController!.SearchBar.TextChanged -= OnSearchTextChanged;
            NavigationItem.SearchController!.SearchBar.CancelButtonClicked -=
                OnSearchCancelButtonClicked;
        }

        base.Dispose(disposing);
    }
}
