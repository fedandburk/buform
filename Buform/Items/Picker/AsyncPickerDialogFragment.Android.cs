using Android.Runtime;

namespace Buform;

public class AsyncPickerDialogFragment : PickerDialogFragment
{
    private readonly IAsyncPickerFormItem? _item;

    private CancellationTokenSource? _cancellationTokenSource;

    public AsyncPickerDialogFragment()
    {
        /* Required constructor */
    }

    protected AsyncPickerDialogFragment(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public AsyncPickerDialogFragment(IAsyncPickerFormItem item)
        : base(item)
    {
        _item = item;
    }

    public override void OnStart()
    {
        base.OnStart();

        LoadOptions();
    }

    public override void OnStop()
    {
        CancelLoading();

        base.OnStop();
    }

    private async void LoadOptions()
    {
        if (_item is not { State: AsyncPickerLoadingState.None })
        {
            return;
        }

        CancelLoading();

        _cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = _cancellationTokenSource.Token;

        try
        {
            await _item.LoadOptionsAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
    }

    private void CancelLoading()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    protected override void UpdateSearchVisibility(int optionCount)
    {
        if (_item?.State != AsyncPickerLoadingState.Loaded)
        {
            SearchLayout?.Visibility = Android.Views.ViewStates.Gone;

            return;
        }

        base.UpdateSearchVisibility(optionCount);
    }

    protected override void UpdateState(int optionCount)
    {
        var state = _item?.State ?? AsyncPickerLoadingState.None;

        switch (state)
        {
            case AsyncPickerLoadingState.None:
            case AsyncPickerLoadingState.Loading:
                ShowState(FormTexts.Loading, true);
                break;
            case AsyncPickerLoadingState.Loaded:
                base.UpdateState(optionCount);
                break;
            case AsyncPickerLoadingState.Failed:
                ShowState(FormTexts.LoadingFailed, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
