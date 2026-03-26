using Android.Text;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;

namespace Buform;

public sealed class PickerDialogFragment : AndroidX.Fragment.App.DialogFragment
{
    private readonly List<IPickerOptionFormItem> _filteredOptions = [];

    private IPickerFormItemBase? _item;
    private PickerOptionsAdapter? _adapter;

    private MaterialTextView? _titleView;
    private MaterialButton? _clearButton;
    private RecyclerView? _recyclerView;
    private MaterialTextView? _helperTextView;
    private TextInputLayout? _searchLayout;
    private TextInputEditText? _searchInput;
    private MaterialButton? _doneButton;

    public static PickerDialogFragment Create(IPickerFormItemBase item)
    {
        return new PickerDialogFragment { _item = item };
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
        return inflater.Inflate(Resource.Layout.PickerDialogLayout, container, false);
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
        _doneButton = view.FindViewById<MaterialButton>(Resource.Id.DoneButton);

        BindItem();
        SetupRecyclerView();
        SetupSearch();
        SetupButtons();
    }

    private void BindItem()
    {
        if (_item == null)
        {
            return;
        }

        _titleView!.Text = _item.Label ?? string.Empty;
        _helperTextView!.Text = _item.Message ?? string.Empty;
        _helperTextView.Visibility = string.IsNullOrWhiteSpace(_item.Message)
            ? ViewStates.Gone
            : ViewStates.Visible;

        _searchInput!.Text = _item.FilterQuery ?? string.Empty;
        RefreshFilteredOptions();

        _clearButton!.Visibility = _item.CanBeCleared ? ViewStates.Visible : ViewStates.Gone;
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

        if (_doneButton != null)
        {
            _doneButton.Click += OnDoneClicked;
        }

        UpdateDoneButtonVisibility();
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

    private void OnDoneClicked(object? sender, EventArgs e)
    {
        DismissAllowingStateLoss();
    }

    private void OnOptionSelected(IPickerOptionFormItem option)
    {
        if (_item == null)
        {
            return;
        }

        _item.Pick(option);

        if (_item is IMultiValuePickerFormItem)
        {
            _adapter?.NotifyDataSetChanged();
            return;
        }

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

        _searchLayout.Visibility = _item.Options.Skip(1).Any()
            ? ViewStates.Visible
            : ViewStates.Gone;
    }

    private void UpdateDoneButtonVisibility()
    {
        if (_doneButton == null)
        {
            return;
        }

        _doneButton.Visibility =
            _item is IMultiValuePickerFormItem ? ViewStates.Visible : ViewStates.Gone;
    }

    public override void OnDestroyView()
    {
        if (_searchInput != null)
        {
            _searchInput.TextChanged -= OnSearchTextChanged;
        }

        if (_doneButton != null)
        {
            _doneButton.Click -= OnDoneClicked;
        }

        if (_clearButton != null)
        {
            _clearButton.Click -= OnClearClicked;
        }

        _recyclerView?.SetAdapter(null);
        _adapter = null;

        _titleView = null;
        _doneButton = null;
        _clearButton = null;
        _recyclerView = null;
        _helperTextView = null;
        _searchLayout = null;
        _searchInput = null;

        base.OnDestroyView();
    }
}
