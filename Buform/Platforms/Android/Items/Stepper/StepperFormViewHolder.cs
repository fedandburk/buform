using Android.Runtime;
using Android.Views;

namespace Buform;

[Preserve(AllMembers = true)]
public class StepperFormViewHolder : FormViewHolder<StepperFormItem>
{
    private TextView? _valueText;
    private ImageButton? _minusButton;
    private ImageButton? _plusButton;

    private bool _isUpdatingFromModel;

    public StepperFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public StepperFormViewHolder(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        _valueText = ItemView.FindViewById<TextView>(Resource.Id.ValueText)!;
        _minusButton = ItemView.FindViewById<ImageButton>(Resource.Id.MinusButton)!;
        _plusButton = ItemView.FindViewById<ImageButton>(Resource.Id.PlusButton)!;

        AttachListeners();
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateValue();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(StepperFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(StepperFormItem.Value):
            case nameof(StepperFormItem.Label):
                UpdateValue();
                break;
            default:
                UpdateReadOnlyState();
                UpdateValue();
                break;
        }
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_valueText == null || _minusButton == null || _plusButton == null)
        {
            return;
        }

        var isReadOnly = Data?.IsReadOnly ?? true;

        _minusButton.Enabled = !isReadOnly;
        _plusButton.Enabled = !isReadOnly;
    }

    protected virtual void UpdateValue()
    {
        if (_valueText == null || Data == null)
        {
            return;
        }

        var value = Data.Label ?? Data.Value.ToString();

        if (string.Equals(_valueText.Text, value, StringComparison.Ordinal))
        {
            return;
        }

        _isUpdatingFromModel = true;
        _valueText.Text = value;
        _isUpdatingFromModel = false;
    }

    private void AttachListeners()
    {
        if (_minusButton != null)
        {
            _minusButton.Click += OnMinusClicked;
        }

        if (_plusButton != null)
        {
            _plusButton.Click += OnPlusClicked;
        }
    }

    private void DetachListeners()
    {
        if (_minusButton != null)
        {
            _minusButton.Click -= OnMinusClicked;
        }

        if (_plusButton != null)
        {
            _plusButton.Click -= OnPlusClicked;
        }
    }

    private void OnMinusClicked(object? sender, EventArgs e)
    {
        if (_isUpdatingFromModel || Data == null)
        {
            return;
        }

        var newValue = Data.Value - Data.StepAmount;

        if (newValue < Data.MinValue)
        {
            newValue = Data.MaxValue;
        }

        Data.Value = newValue;
    }

    private void OnPlusClicked(object? sender, EventArgs e)
    {
        if (_isUpdatingFromModel || Data == null)
        {
            return;
        }

        var newValue = Data.Value + Data.StepAmount;

        if (newValue > Data.MaxValue)
        {
            newValue = Data.MinValue;
        }

        Data.Value = newValue;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachListeners();

            _valueText = null;
            _minusButton = null;
            _plusButton = null;
        }

        base.Dispose(disposing);
    }
}
