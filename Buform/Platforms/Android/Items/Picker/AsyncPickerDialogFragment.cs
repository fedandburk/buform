using Android.Text;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using OperationCanceledException = System.OperationCanceledException;

namespace Buform;

public sealed class AsyncPickerDialogFragment : PickerDialogFragment
{
    private readonly List<IPickerOptionFormItem> _filteredOptions = [];

    private IAsyncPickerFormItem? _item;
    private PickerOptionsAdapter? _adapter;
    private CancellationTokenSource? _loadCancellationTokenSource;

    private MaterialTextView? _titleView;
    private MaterialButton? _clearButton;
    private RecyclerView? _recyclerView;
    private MaterialTextView? _helperTextView;
    private TextInputLayout? _searchLayout;
    private TextInputEditText? _searchInput;

    private CircularProgressIndicator? _progressIndicator;
    private MaterialTextView? _stateTextView;
    private View? _contentContainer;

    public static AsyncPickerDialogFragment Create(IAsyncPickerFormItem item)
    {
        return new AsyncPickerDialogFragment { _item = item };
    }

    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        var dialog = base.OnCreateDialog(savedInstanceState);
        dialog.Window?.SetLayout(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent
        );

        return dialog;
    }

    public override View OnCreateView(
        LayoutInflater inflater,
        ViewGroup? container,
        Bundle? savedInstanceState
    )
    {
        return inflater.Inflate(Resource.Layout.AsyncPickerDialogLayout, container, false);
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        _titleView = view.FindViewById<MaterialTextView>(Resource.Id.Title);
        _clearButton = view.FindViewById<MaterialButton>(Resource.Id.ClearButton);
        _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.OptionsRecyclerView);
        _helperTextView = view.FindViewById<MaterialTextView>(Resource.Id.HelperText);
        _searchLayout = view.FindViewById<TextInputLayout>(Resource.Id.SearchLayout);
        _searchInput = view.FindViewById<TextInputEditText>(Resource.Id.SearchInput);

        _progressIndicator = view.FindViewById<CircularProgressIndicator>(
            Resource.Id.ProgressIndicator
        );
        _stateTextView = view.FindViewById<MaterialTextView>(Resource.Id.StateText);
        _contentContainer = view.FindViewById(Resource.Id.ContentContainer);

        BindItem();
        SetupRecyclerView();
        SetupSearch();
        SetupButtons();
        UpdateState();

        _ = LoadItemsAsync();
    }

    private void BindItem()
    {
        if (_item == null)
        {
            return;
        }

        if (_titleView != null)
        {
            _titleView.Text = _item.Label ?? string.Empty;
        }

        if (_helperTextView != null)
        {
            _helperTextView.Text = _item.Message ?? string.Empty;
            _helperTextView.Visibility = string.IsNullOrWhiteSpace(_item.Message)
                ? ViewStates.Gone
                : ViewStates.Visible;
        }

        if (_searchInput != null)
        {
            _searchInput.Text = _item.FilterQuery ?? string.Empty;
        }

        RefreshFilteredOptions();

        if (_clearButton != null)
        {
            _clearButton.Visibility = _item.CanBeCleared ? ViewStates.Visible : ViewStates.Gone;
        }
    }

    private void SetupRecyclerView()
    {
        if (_item == null || _recyclerView == null || Context == null)
        {
            return;
        }

        _adapter = new PickerOptionsAdapter(
            _filteredOptions,
            OnOptionSelected,
            option => _item.IsPicked(option)
        );

        _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        _recyclerView.SetAdapter(_adapter);
    }

    private void SetupSearch()
    {
        if (_searchInput == null)
        {
            return;
        }

        _searchInput.TextChanged += OnSearchTextChanged;
        UpdateSearchVisibility();
    }

    private void SetupButtons()
    {
        if (_clearButton != null)
        {
            _clearButton.Click += OnClearClicked;
        }
    }

    private async Task LoadItemsAsync()
    {
        if (_item == null)
        {
            return;
        }

        if (_item.State == AsyncPickerLoadingState.Loaded && _item.Options.Any())
        {
            UpdateState();
            return;
        }

        _loadCancellationTokenSource?.Cancel();
        _loadCancellationTokenSource?.Dispose();
        _loadCancellationTokenSource = new CancellationTokenSource();

        UpdateState();

        try
        {
            await _item.LoadItemsAsync(_loadCancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            // The item is expected to update its own State to Failed.
            System.Diagnostics.Debug.WriteLine($"LoadItemsAsync failed: {ex.Message}");
        }

        if (!IsAdded)
        {
            return;
        }

        Activity?.RunOnUiThread(() =>
        {
            RefreshFilteredOptions();
            _adapter?.NotifyDataSetChanged();
            UpdateSearchVisibility();
            UpdateState();
        });
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        if (_item == null)
        {
            return;
        }

        _item.Pick(null);
        DismissAllowingStateLoss();
    }

    private void OnOptionSelected(IPickerOptionFormItem option)
    {
        if (_item == null)
        {
            return;
        }

        _item.Pick(option);
        DismissAllowingStateLoss();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_item == null)
        {
            return;
        }

        _item.FilterQuery = e.Text?.ToString();
        RefreshFilteredOptions();
        _adapter?.NotifyDataSetChanged();
    }

    private void RefreshFilteredOptions()
    {
        if (_item == null)
        {
            return;
        }

        _filteredOptions.Clear();

        var query = _item.FilterQuery?.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            _filteredOptions.AddRange(_item.Options);
            return;
        }

        _filteredOptions.AddRange(
            _item.Options.Where(option =>
                (option.FormattedValue ?? string.Empty).Contains(
                    query,
                    StringComparison.OrdinalIgnoreCase
                )
            )
        );
    }

    private void UpdateSearchVisibility()
    {
        if (_searchLayout == null || _item == null)
        {
            return;
        }

        var isLoaded = _item.State == AsyncPickerLoadingState.Loaded;
        var hasItems = _item.Options.Any();

        _searchLayout.Visibility =
            isLoaded && hasItems && _item.Options.Skip(1).Any()
                ? ViewStates.Visible
                : ViewStates.Gone;
    }

    private void UpdateState()
    {
        if (
            _item == null
            || _progressIndicator == null
            || _stateTextView == null
            || _contentContainer == null
        )
        {
            return;
        }

        switch (_item.State)
        {
            case AsyncPickerLoadingState.None:
            case AsyncPickerLoadingState.Loading:
                _progressIndicator.Visibility = ViewStates.Visible;
                _stateTextView.Visibility = ViewStates.Visible;
                _stateTextView.Text = "Loading...";
                _contentContainer.Visibility = ViewStates.Gone;
                break;

            case AsyncPickerLoadingState.Loaded:
                _progressIndicator.Visibility = ViewStates.Gone;

                if (!_item.Options.Any())
                {
                    _stateTextView.Visibility = ViewStates.Visible;
                    _stateTextView.Text = "No items";
                    _contentContainer.Visibility = ViewStates.Gone;
                }
                else
                {
                    _stateTextView.Visibility = ViewStates.Gone;
                    _contentContainer.Visibility = ViewStates.Visible;
                }

                break;

            case AsyncPickerLoadingState.Failed:
                _progressIndicator.Visibility = ViewStates.Gone;
                _stateTextView.Visibility = ViewStates.Visible;
                _stateTextView.Text = "Failed to load items";
                _contentContainer.Visibility = ViewStates.Gone;
                break;

            default:
                _progressIndicator.Visibility = ViewStates.Gone;
                _stateTextView.Visibility = ViewStates.Gone;
                _contentContainer.Visibility = ViewStates.Visible;
                break;
        }
    }

    public override void OnDestroyView()
    {
        _loadCancellationTokenSource?.Cancel();
        _loadCancellationTokenSource?.Dispose();
        _loadCancellationTokenSource = null;

        if (_searchInput != null)
        {
            _searchInput.TextChanged -= OnSearchTextChanged;
        }

        if (_clearButton != null)
        {
            _clearButton.Click -= OnClearClicked;
        }

        _recyclerView?.SetAdapter(null);
        _adapter = null;

        _titleView = null;
        _clearButton = null;
        _recyclerView = null;
        _helperTextView = null;
        _searchLayout = null;
        _searchInput = null;
        _progressIndicator = null;
        _stateTextView = null;
        _contentContainer = null;

        base.OnDestroyView();
    }
}
