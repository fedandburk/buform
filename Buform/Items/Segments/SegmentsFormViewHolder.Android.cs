using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.Button;

namespace Buform;

public abstract class SegmentsFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : class, ISegmentsFormItem
{
    private const float ButtonWeight = 1;

    private readonly List<IFormOption> _options;

    private SegmentsButtonCheckedListener? _buttonCheckedListener;
    private bool _isUpdatingValue;

    protected SegmentsFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        _options = [];
    }

    protected SegmentsFormViewHolder(View itemView)
        : base(itemView)
    {
        _options = [];
    }

    protected virtual TextView? LabelView { get; set; }

    protected virtual MaterialButtonToggleGroup? ToggleGroupView { get; set; }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);
        ToggleGroupView = ItemView.FindViewById<MaterialButtonToggleGroup>(Resource.Id.ToggleGroup);
        ValidationErrorView = ItemView.FindViewById<TextView>(Resource.Id.Error);

        _buttonCheckedListener = new SegmentsButtonCheckedListener(OnButtonChecked);

        ToggleGroupView?.AddOnButtonCheckedListener(_buttonCheckedListener);
    }

    protected virtual MaterialButton CreateButton(IFormOption option)
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
        if (ToggleGroupView == null)
        {
            return;
        }

        var isEnabled = !(Item?.IsReadOnly ?? true);

        LabelView?.Enabled = isEnabled;
        ToggleGroupView.Enabled = isEnabled;

        for (var index = 0; index < ToggleGroupView.ChildCount; index++)
        {
            ToggleGroupView.GetChildAt(index)?.Enabled = isEnabled;
        }
    }

    protected virtual void UpdateLabel()
    {
        var label = Item?.Label;

        LabelView?.Text = label;
        LabelView?.Visibility = string.IsNullOrEmpty(label) ? ViewStates.Gone : ViewStates.Visible;
    }

    protected virtual void UpdateOptions()
    {
        if (ToggleGroupView == null)
        {
            return;
        }

        _options.Clear();
        _options.AddRange(Item?.Options ?? []);

        ToggleGroupView.RemoveAllViews();

        foreach (var option in _options)
        {
            var button = CreateButton(option);

            button.SetSingleLine(true);

            ToggleGroupView.AddView(button);
        }

        UpdateReadOnlyState();
    }

    protected virtual void UpdateValue()
    {
        if (ToggleGroupView == null || Item == null)
        {
            return;
        }

        var index = _options.FindIndex(option => Equals(option.Value, Item.Value));
        var button = index < 0 ? null : ToggleGroupView.GetChildAt(index);

        _isUpdatingValue = true;

        try
        {
            if (button == null)
            {
                ToggleGroupView.ClearChecked();
            }
            else
            {
                ToggleGroupView.Check(button.Id);
            }
        }
        finally
        {
            _isUpdatingValue = false;
        }
    }

    protected override void OnItemSet()
    {
        UpdateLabel();
        UpdateOptions();
        UpdateReadOnlyState();
        UpdateValue();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ISegmentsFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ISegmentsFormItem.Label):
                UpdateLabel();
                break;
            case nameof(ISegmentsFormItem.Options):
                UpdateOptions();
                UpdateValue();
                break;
            case nameof(ISegmentsFormItem.Value):
                UpdateValue();
                break;
            case null:
                UpdateLabel();
                UpdateOptions();
                UpdateReadOnlyState();
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
                ToggleGroupView?.RemoveOnButtonCheckedListener(_buttonCheckedListener);
            }

            _buttonCheckedListener?.Dispose();
            _buttonCheckedListener = null;

            LabelView = null;
            ToggleGroupView = null;
        }

        base.Dispose(disposing);
    }

    private void OnButtonChecked(int checkedId, bool isChecked)
    {
        if (_isUpdatingValue || !isChecked || ToggleGroupView == null || Item == null)
        {
            return;
        }

        var button = ToggleGroupView.FindViewById(checkedId)
            ?? throw new InvalidOperationException("The checked segment is not found.");

        var index = ToggleGroupView.IndexOfChild(button);

        Item.Value = _options[index].Value;
    }
}

public sealed class SegmentsFormViewHolder : SegmentsFormViewHolder<ISegmentsFormItem>
{
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
}
