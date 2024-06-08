namespace Buform;

[Preserve(AllMembers = true)]
public sealed class MenuViewController : UITableViewController, INavigationService
{
    private readonly MenuViewModel _viewModel;

    private UITableViewSource? _source;

    public MenuViewController()
        : base(UITableViewStyle.InsetGrouped)
    {
        _viewModel = new MenuViewModel(this);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Buform";

        TableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

        _source = new FormTableViewSource(TableView) { Form = _viewModel.Form };
    }

    public Task OpenComponentsAsync()
    {
        return PresentViewControllerAsync(
            new UINavigationController(new ComponentsViewController()),
            true
        );
    }

    public Task OpenCreateConnectionAsync()
    {
        return PresentViewControllerAsync(
            new UINavigationController(new CreateConnectionViewController()),
            true
        );
    }

    public Task OpenCreateEventAsync()
    {
        return PresentViewControllerAsync(
            new UINavigationController(new CreateEventViewController()),
            true
        );
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        _source?.Dispose();
    }
}
