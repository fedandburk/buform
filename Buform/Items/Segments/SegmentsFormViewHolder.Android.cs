using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.Button;
using Object = Java.Lang.Object;

namespace Buform;

public class SegmentsFormViewHolder : FormViewHolder<ISegmentsFormItem>
{
    private const float ButtonWeight = 1;

    private readonly List<ISegmentsOptionFormItem> _options;

    private ButtonCheckedListener? _buttonCheckedListener;
    private bool _isUpdatingValue;

    protected virtual TextView? LabelView { get; set; }

    protected virtual MaterialButtonToggleGroup? ToggleGroup { get; set; }

    protected virtual TextView? ErrorView { get; set; }

    protected override TextView? ValidationErrorView => ErrorView;

    public SegmentsFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        _options = [];
    }

    public SegmentsFormViewHolder(View itemView)
        : base(itemView)
    {
        _options = [];
    }

    private void OnButtonChecked(int checkedId, bool isChecked)
    {
        if (_isUpdatingValue || !isChecked || ToggleGroup == null || Data == null)
        {
            return;
        }

        var button = ToggleGroup.FindViewById(checkedId)
            ?? throw new InvalidOperationException("The checked segment is not found.");

        var index = ToggleGroup.IndexOfChild(button);

        Data.Value = _options[index].Value;
    }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        ToggleGroup = ItemView.FindViewById<MaterialButtonToggleGroup>(Resource.Id.ToggleGroup);
        ErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        _buttonCheckedListener = new ButtonCheckedListener(this);

        ToggleGroup?.AddOnButtonCheckedListener(_buttonCheckedListener);
    }

    protected virtual MaterialButton CreateButton(ISegmentsOptionFormItem option)
    {
        ArgumentNullException.ThrowIfNull(option);

        var context = ItemView.Context
            ?? throw new InvalidOperationException("The view has no context.");

        return new MaterialButton(context, null, Resource.Attribute.materialButtonOutlinedStyle)
        {
            Id = View.GenerateViewId(),
            Text = option.FormattedValue,
            Ellipsize = TextUtils.TruncateAt.End,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent,
                ButtonWeight
            ),
        };
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (ToggleGroup == null)
        {
            return;
        }

        var isEnabled = !(Data?.IsReadOnly ?? true);

        LabelView?.Enabled = isEnabled;
        ToggleGroup.Enabled = isEnabled;

        for (var index = 0; index < ToggleGroup.ChildCount; index++)
        {
            ToggleGroup.GetChildAt(index)?.Enabled = isEnabled;
        }
    }

    protected virtual void UpdateLabel()
    {
        var label = Data?.Label;

        LabelView?.Text = label;
        LabelView?.Visibility = string.IsNullOrEmpty(label) ? ViewStates.Gone : ViewStates.Visible;
    }

    protected virtual void UpdateItems()
    {
        if (ToggleGroup == null)
        {
            return;
        }

        _options.Clear();
        _options.AddRange(Data?.Items ?? []);

        ToggleGroup.RemoveAllViews();

        foreach (var option in _options)
        {
            var button = CreateButton(option);

            button.SetSingleLine(true);

            ToggleGroup.AddView(button);
        }

        UpdateReadOnlyState();
    }

    protected virtual void UpdateValue()
    {
        if (ToggleGroup == null || Data == null)
        {
            return;
        }

        var index = _options.FindIndex(option => Equals(option.Value, Data.Value));
        var button = index < 0 ? null : ToggleGroup.GetChildAt(index);

        _isUpdatingValue = true;

        try
        {
            if (button == null)
            {
                ToggleGroup.ClearChecked();
            }
            else
            {
                ToggleGroup.Check(button.Id);
            }
        }
        finally
        {
            _isUpdatingValue = false;
        }
    }

    protected override void OnDataSet()
    {
        UpdateLabel();
        UpdateItems();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ISegmentsFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ISegmentsFormItem.Label):
                UpdateLabel();
                break;
            case nameof(ISegmentsFormItem.Items):
                UpdateItems();
                UpdateValue();
                break;
            case nameof(ISegmentsFormItem.Value):
                UpdateValue();
                break;
            case null:
                UpdateLabel();
                UpdateItems();
                UpdateValue();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_buttonCheckedListener != null)
            {
                ToggleGroup?.RemoveOnButtonCheckedListener(_buttonCheckedListener);
            }

            _buttonCheckedListener?.Dispose();
            _buttonCheckedListener = null;

            LabelView = null;
            ToggleGroup = null;
            ErrorView = null;
        }

        base.Dispose(disposing);
    }

    private sealed class ButtonCheckedListener
        : Object,
            MaterialButtonToggleGroup.IOnButtonCheckedListener
    {
        private readonly SegmentsFormViewHolder _owner;

        public ButtonCheckedListener(SegmentsFormViewHolder owner)
        {
            _owner = owner;
        }

        public void OnButtonChecked(MaterialButtonToggleGroup? group, int checkedId, bool isChecked)
        {
            _owner.OnButtonChecked(checkedId, isChecked);
        }
    }
}
