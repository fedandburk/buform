using Fedandburk.Common.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class ComponentsViewController : UITableViewController
{
    private readonly ComponentsViewModel _viewModel;

    private UIBarButtonItem? _cancelButtonItem;
    private UIBarButtonItem? _toggleReadOnlyButtonItem;
    private UITableViewSource? _source;

    public ComponentsViewController()
        : base(UITableViewStyle.InsetGrouped)
    {
        _viewModel = new ComponentsViewModel();
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Components";

        _cancelButtonItem = new UIBarButtonItem(
            UIBarButtonSystemItem.Cancel,
            (_, _) => DismissViewController(true, null)
        );

        _toggleReadOnlyButtonItem = new UIBarButtonItem(
            "Read-only",
            UIBarButtonItemStyle.Plain,
            (_, _) => _viewModel.ToggleReadOnlyModeCommand.SafeExecute()
        );

        NavigationItem.LeftBarButtonItem = _cancelButtonItem;
        NavigationItem.RightBarButtonItems = [EditButtonItem, _toggleReadOnlyButtonItem];

        TableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

        _source = new FormTableViewSource(TableView) { Form = _viewModel.Form };
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        _cancelButtonItem?.Dispose();
        _toggleReadOnlyButtonItem?.Dispose();
        _source?.Dispose();
    }
}
