using Android.Runtime;
using Android.Views;
using Google.Android.Material.Button;

namespace Buform;

[Preserve(AllMembers = true)]
public class SegmentsFormViewHolder : FormViewHolder<ISegmentsFormItem>
{
    private TextView? _label;
    private MaterialButtonToggleGroup? _toggleGroup;

    private List<ISegmentsOptionFormItem> _items = [];
    private bool _isUpdatingFromModel;

    public SegmentsFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public SegmentsFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        _label = ItemView.FindViewById<TextView>(Resource.Id.Label)!;
        _toggleGroup = ItemView.FindViewById<MaterialButtonToggleGroup>(Resource.Id.ToggleGroup)!;

        AttachListeners();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateItems();
        UpdateValue();
        UpdateValidationErrorMessage();
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
            case nameof(ISegmentsFormItem.ValidationErrorMessage):
                UpdateValidationErrorMessage();
                break;
            default:
                UpdateReadOnlyState();
                UpdateLabel();
                UpdateItems();
                UpdateValue();
                UpdateValidationErrorMessage();
                break;
        }
    }

    private void AttachListeners()
    {
        if (_toggleGroup == null)
        {
            return;
        }

        _toggleGroup.AddOnButtonCheckedListener(new ButtonCheckedListener(this));
    }

    private void OnButtonChecked(int checkedId, bool isChecked)
    {
        if (_isUpdatingFromModel || !isChecked || _toggleGroup == null || Data == null)
        {
            return;
        }

        var button = _toggleGroup.FindViewById<MaterialButton>(checkedId);

        if (button?.Tag is not Java.Lang.Integer javaInt)
        {
            Data.Value = null;
            return;
        }

        var index = javaInt.IntValue();

        Data.Value = _items.ElementAtOrDefault(index)?.Value;
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_toggleGroup == null)
        {
            return;
        }

        var isEnabled = !(Data?.IsReadOnly ?? true);

        _toggleGroup.Enabled = isEnabled;

        for (var i = 0; i < _toggleGroup.ChildCount; i++)
        {
            var child = _toggleGroup.GetChildAt(i);
            child.Enabled = isEnabled;
        }
    }

    protected virtual void UpdateLabel()
    {
        if (_label == null)
        {
            return;
        }

        var hasLabel = !string.IsNullOrWhiteSpace(Data?.Label);

        _label.Text = Data?.Label;
        _label.Visibility = hasLabel ? ViewStates.Visible : ViewStates.Gone;
    }

    protected virtual void UpdateItems()
    {
        if (_toggleGroup == null)
        {
            return;
        }

        _items = Data?.Items?.ToList() ?? new List<ISegmentsOptionFormItem>();

        _toggleGroup.RemoveAllViews();

        var context = _toggleGroup.Context;

        for (var index = 0; index < _items.Count; index++)
        {
            var option = _items[index];

            var button = new MaterialButton(
                context,
                null,
                Resource.Attribute.materialButtonOutlinedStyle
            )
            {
                Id = View.GenerateViewId(),
                Tag = index
            };

            button.Text = option.FormattedValue ?? string.Empty;
            button.SetSingleLine(true);
            button.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
            button.LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent,
                1f
            );

            _toggleGroup.AddView(button);
        }

        UpdateReadOnlyState();
    }

    protected virtual void UpdateValue()
    {
        if (_toggleGroup == null)
        {
            return;
        }

        _isUpdatingFromModel = true;

        try
        {
            _toggleGroup.ClearChecked();

            if (_items.Count == 0 || Data?.Value == null)
            {
                return;
            }

            var index = _items.FindIndex(item => Equals(item.Value, Data.Value));
            if (index < 0 || index >= _toggleGroup.ChildCount)
            {
                return;
            }

            var button = _toggleGroup.GetChildAt(index);
            if (button != null)
            {
                _toggleGroup.Check(button.Id);
            }
        }
        finally
        {
            _isUpdatingFromModel = false;
        }
    }

    protected virtual void UpdateValidationErrorMessage()
    {
        if (_label == null)
        {
            return;
        }

        var hasError = !string.IsNullOrWhiteSpace(Data?.ValidationErrorMessage);

        var typedValue = new Android.Util.TypedValue();
        var attribute = hasError
            ? Resource.Attribute.colorError
            : Resource.Attribute.colorOnSurface;
        if (_label.Context?.Theme?.ResolveAttribute(attribute, typedValue, true) == true)
        {
            _label.SetTextColor(new Android.Graphics.Color(typedValue.Data));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _label = null;
            _toggleGroup = null;
        }

        base.Dispose(disposing);
    }

    private sealed class ButtonCheckedListener
        : Java.Lang.Object,
            MaterialButtonToggleGroup.IOnButtonCheckedListener
    {
        private readonly SegmentsFormViewHolder _owner;

        public ButtonCheckedListener(SegmentsFormViewHolder owner)
        {
            _owner = owner;
        }

        public void OnButtonChecked(MaterialButtonToggleGroup? group, int checkedId, bool isChecked)
        {
            _ = group;
            _owner.OnButtonChecked(checkedId, isChecked);
        }
    }
}
