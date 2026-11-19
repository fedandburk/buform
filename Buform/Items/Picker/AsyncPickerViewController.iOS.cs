namespace Buform;

public abstract class AsyncPickerViewController<TItem> : PickerViewController<TItem>
    where TItem : class, IAsyncPickerFormItem
{
    protected CancellationTokenSource? CancellationTokenSource { get; private set; }

    protected AsyncPickerViewController(UITableViewStyle style, TItem item)
        : base(style, item)
    {
        /* Required constructor */
    }

    protected virtual async Task LoadItemsIfNeeded()
    {
        if (Item == null)
        {
            return;
        }

        if (Item.State == AsyncPickerLoadingState.Loaded)
        {
            return;
        }

        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = new CancellationTokenSource();

        var activityIndicator = new UIActivityIndicatorView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Medium,
            HidesWhenStopped = true,
        };

        View!.AddSubview(activityIndicator);

        View.AddConstraints(
            new[]
            {
                activityIndicator.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
                activityIndicator.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor),
            }
        );

        activityIndicator.StartAnimating();

        try
        {
            await Item.LoadItemsAsync(CancellationTokenSource.Token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            activityIndicator.StopAnimating();
            activityIndicator.RemoveFromSuperview();
            activityIndicator.Dispose();
        }

        TableView.ReloadData();
    }

    public override async void ViewDidLoad()
    {
        base.ViewDidLoad();

        await LoadItemsIfNeeded().ConfigureAwait(true);
    }

    public override string TitleForFooter(UITableView tableView, nint section)
    {
        return Item?.State != AsyncPickerLoadingState.Loaded
            ? string.Empty
            : Item?.Message ?? string.Empty;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }

        base.Dispose(disposing);
    }
}

public sealed class AsyncPickerViewController : AsyncPickerViewController<IAsyncPickerFormItem>
{
    public AsyncPickerViewController(UITableViewStyle style, IAsyncPickerFormItem item)
        : base(style, item)
    {
        /* Required constructor */
    }
}
