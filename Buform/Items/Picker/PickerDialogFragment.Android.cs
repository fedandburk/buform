using System.ComponentModel;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;

namespace Buform;

public class PickerDialogFragment : AppCompatDialogFragment
{
    private PickerOptionsAdapter? _adapter;
    private bool _isSearchVisible;

    protected IOptionsPickerFormItem? Item { get; }

    protected virtual TextView? MessageView { get; set; }

    protected virtual TextInputLayout? SearchLayout { get; set; }

    protected virtual EditText? SearchInput { get; set; }

    protected virtual View? StateContainer { get; set; }

    protected virtual View? ProgressIndicator { get; set; }

    protected virtual TextView? StateTextView { get; set; }

    protected virtual RecyclerView? OptionsView { get; set; }

    protected virtual bool IsMultipleChoice => Item is IMultiValuePickerFormItem;

    public PickerDialogFragment()
    {
        /* Required constructor */
    }

    protected PickerDialogFragment(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public PickerDialogFragment(IOptionsPickerFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Item = item;
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Refresh();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        Item?.FilterQuery = e.Text?.ToString();
    }

    private void OnOptionClick(IPickerOptionFormItem option)
    {
        if (Item == null)
        {
            return;
        }

        Item.Pick(option);

        if (!IsMultipleChoice)
        {
            Dismiss();

            return;
        }

        Refresh();
    }

    private void OnClearClick(object? sender, DialogClickEventArgs e)
    {
        Item?.Pick(null);
    }

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Item == null)
        {
            DismissAllowingStateLoss();
        }
    }

    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        if (Item == null)
        {
            return base.OnCreateDialog(savedInstanceState);
        }

        var context = RequireContext();
        var view = CreateContentView(context);

        var builder = new MaterialAlertDialogBuilder(context);

        builder.SetTitle(Item.Label);
        builder.SetView(view);

        if (IsMultipleChoice)
        {
            builder.SetPositiveButton(FormTexts.Done, (_, _) => { });
        }
        else
        {
            builder.SetNegativeButton(FormTexts.Cancel, (_, _) => { });
        }

        if (Item.CanBeCleared)
        {
            builder.SetNeutralButton(FormTexts.Clear, OnClearClick);
        }

        return builder.Create();
    }

    protected virtual View CreateContentView(Context context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var inflater = LayoutInflater.From(context)
            ?? throw new InvalidOperationException("The context has no layout inflater.");

        var view = inflater.Inflate(Resource.Layout.PickerDialogLayout, null, false)
            ?? throw new InvalidOperationException("The picker dialog layout is not inflated.");

        MessageView = view.FindViewById<TextView>(Resource.Id.Message);
        SearchLayout = view.FindViewById<TextInputLayout>(Resource.Id.SearchLayout);
        SearchInput = view.FindViewById<EditText>(Resource.Id.SearchInput);
        StateContainer = view.FindViewById(Resource.Id.StateContainer);
        ProgressIndicator = view.FindViewById(Resource.Id.ProgressIndicator);
        StateTextView = view.FindViewById<TextView>(Resource.Id.StateText);
        OptionsView = view.FindViewById<RecyclerView>(Resource.Id.OptionsRecyclerView);

        var message = Item?.Message;

        MessageView?.Text = message;
        MessageView?.Visibility = string.IsNullOrEmpty(message)
            ? ViewStates.Gone
            : ViewStates.Visible;

        SearchLayout?.Hint = FormTexts.Search;
        SearchInput?.Text = Item?.FilterQuery;
        SearchInput?.TextChanged += OnSearchTextChanged;

        var layoutId = IsMultipleChoice
            ? Resource.Layout.PickerMultipleOptionLayout
            : Resource.Layout.PickerSingleOptionLayout;

        _adapter = new PickerOptionsAdapter(layoutId, OnOptionClick);

        OptionsView?.SetLayoutManager(new LinearLayoutManager(context));
        OptionsView?.SetAdapter(_adapter);

        return view;
    }

    public override void OnStart()
    {
        base.OnStart();

        if (Item == null)
        {
            return;
        }

        Item.PropertyChanged += OnItemPropertyChanged;

        Refresh();
    }

    public override void OnStop()
    {
        Item?.PropertyChanged -= OnItemPropertyChanged;

        base.OnStop();
    }

    public override void OnDismiss(IDialogInterface dialog)
    {
        Item?.FilterQuery = null;

        base.OnDismiss(dialog);
    }

    protected virtual void Refresh()
    {
        if (Item == null || _adapter == null)
        {
            return;
        }

        var options = Item
            .Options.Select(option => new PickerOptionState(
                option,
                option.FormattedValue,
                Item.IsPicked(option)
            ))
            .ToList();

        _adapter.SetOptions(options);

        UpdateSearchVisibility(options.Count);
        UpdateState(options.Count);
    }

    protected virtual void UpdateSearchVisibility(int optionCount)
    {
        _isSearchVisible = _isSearchVisible || optionCount > 1;

        SearchLayout?.Visibility = _isSearchVisible ? ViewStates.Visible : ViewStates.Gone;
    }

    protected virtual void UpdateState(int optionCount)
    {
        if (optionCount > 0 || !string.IsNullOrEmpty(Item?.FilterQuery))
        {
            ShowOptions();

            return;
        }

        ShowState(FormTexts.NoItems, false);
    }

    protected void ShowOptions()
    {
        StateContainer?.Visibility = ViewStates.Gone;
        OptionsView?.Visibility = ViewStates.Visible;
    }

    protected void ShowState(string text, bool isLoading)
    {
        ArgumentNullException.ThrowIfNull(text);

        StateContainer?.Visibility = ViewStates.Visible;
        ProgressIndicator?.Visibility = isLoading ? ViewStates.Visible : ViewStates.Gone;
        StateTextView?.Text = text;
        OptionsView?.Visibility = ViewStates.Gone;
    }

    public override void OnDestroy()
    {
        SearchInput?.TextChanged -= OnSearchTextChanged;

        OptionsView?.SetAdapter(null);

        _adapter?.Dispose();
        _adapter = null;

        base.OnDestroy();
    }
}
