using Fedandburk.Common.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class CreateEventViewController : UITableViewController
{
    private readonly CreateEventViewModel _viewModel;

    private UIBarButtonItem? _cancelButtonItem;
    private UIBarButtonItem? _createButtonItem;
    private UITableViewSource? _source;

    public CreateEventViewController()
        : base(UITableViewStyle.InsetGrouped)
    {
        _viewModel = new CreateEventViewModel();
        _viewModel.CreateCommand.CanExecuteChanged += CreateCommandOnCanExecuteChanged;
    }

    private void CreateCommandOnCanExecuteChanged(object? sender, EventArgs e)
    {
        var createButtonItem = _createButtonItem;
        if (createButtonItem != null)
        {
            createButtonItem.Enabled = _viewModel.CreateCommand.SafeCanExecute();
        }
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "New Event";

        _cancelButtonItem = new UIBarButtonItem(
            UIBarButtonSystemItem.Cancel,
            (_, _) => DismissViewController(true, null)
        );

        _createButtonItem = new UIBarButtonItem(
            "Add",
            UIBarButtonItemStyle.Done,
            (_, _) => _viewModel.CreateCommand.SafeExecute()
        );

        _createButtonItem.Enabled = _viewModel.CreateCommand.SafeCanExecute();

        NavigationItem.LeftBarButtonItem = _cancelButtonItem;
        NavigationItem.RightBarButtonItem = _createButtonItem;

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

        _viewModel.CreateCommand.CanExecuteChanged -= CreateCommandOnCanExecuteChanged;

        _cancelButtonItem?.Dispose();
        _createButtonItem?.Dispose();
        _source?.Dispose();
    }
}
