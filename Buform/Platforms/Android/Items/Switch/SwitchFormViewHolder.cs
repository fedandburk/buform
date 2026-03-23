using Android.Runtime;
using Android.Views;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextView;

namespace Buform;

[Preserve(AllMembers = true)]
public class SwitchFormViewHolder : FormViewHolder<SwitchFormItem>
{
    private MaterialSwitch? _switch;
    private MaterialTextView? _title;
    private bool _isUpdatingFromModel;

    public SwitchFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public SwitchFormViewHolder(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        _switch = ItemView.FindViewById<MaterialSwitch>(Resource.Id.Switch)!;
        _title = ItemView.FindViewById<MaterialTextView>(Resource.Id.Title)!;

        AttachListeners();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateTitle();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(SwitchFormItem.Label):
                UpdateTitle();
                break;

            case nameof(SwitchFormItem.Value):
                UpdateValue();
                break;

            case nameof(SwitchFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;

            default:
                UpdateTitle();
                UpdateValue();
                UpdateReadOnlyState();
                break;
        }
    }

    private void AttachListeners()
    {
        if (_switch == null)
        {
            return;
        }

        _switch.CheckedChange += OnCheckedChanged;
        ItemView.Click += OnItemClicked;
    }

    private void DetachListeners()
    {
        if (_switch != null)
        {
            _switch.CheckedChange -= OnCheckedChanged;
        }

        ItemView.Click -= OnItemClicked;
    }

    private void OnItemClicked(object? sender, EventArgs e)
    {
        if (Data?.IsReadOnly ?? true)
        {
            return;
        }

        _switch?.Toggle();
    }

    private void OnCheckedChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        if (_isUpdatingFromModel || Data == null)
        {
            return;
        }

        Data.Value = e.IsChecked;
    }

    private void UpdateTitle()
    {
        if (_title == null)
        {
            return;
        }

        _title.Text = Data?.Label ?? string.Empty;
    }

    private void UpdateValue()
    {
        if (_switch == null || Data == null)
        {
            return;
        }

        if (_switch.Checked == Data.Value)
        {
            return;
        }

        _isUpdatingFromModel = true;

        try
        {
            _switch.Checked = Data.Value;
        }
        finally
        {
            _isUpdatingFromModel = false;
        }
    }

    private void UpdateReadOnlyState()
    {
        if (_switch == null)
        {
            return;
        }

        var isReadOnly = Data?.IsReadOnly ?? true;

        ItemView.Enabled = !isReadOnly;
        ItemView.Clickable = !isReadOnly;
        ItemView.Focusable = !isReadOnly;
        _switch.Enabled = !isReadOnly;
        _switch.Clickable = !isReadOnly;
        _switch.Focusable = !isReadOnly;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachListeners();
            _switch = null;
            _title = null;
        }

        base.Dispose(disposing);
    }
}
