namespace Buform;

[Preserve(AllMembers = true)]
public sealed class CreateConnectionViewController : UITableViewController
{
    private readonly CreateConnectionViewModel _viewModel;

    private UIBarButtonItem? _cancelButtonItem;
    private UITableViewSource? _source;

    public CreateConnectionViewController()
        : base(UITableViewStyle.InsetGrouped)
    {
        _viewModel = new CreateConnectionViewModel();
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "New Connection";

        _cancelButtonItem = new UIBarButtonItem(
            UIBarButtonSystemItem.Cancel,
            (_, _) => DismissViewController(true, null)
        );

        NavigationItem.LeftBarButtonItem = _cancelButtonItem;

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
        _source?.Dispose();
    }
}
