using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.TextField;

namespace Buform;

public class TextInputFormViewHolder : FormViewHolder<ITextInputFormItem>
{
    private bool _isUpdatingValue;

    protected virtual TextInputLayout? InputLayout { get; set; }

    protected virtual EditText? InputView { get; set; }

    public TextInputFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public TextInputFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdatingValue || Data == null)
        {
            return;
        }

        Data.SetValue(e.Text?.ToString());
    }

    protected override void Initialize()
    {
        InputLayout = ItemView.FindViewById<TextInputLayout>(Resource.Id.InputLayout);
        InputView = ItemView.FindViewById<EditText>(Resource.Id.Input);

        InputView?.TextChanged += OnTextChanged;
    }

    protected virtual void UpdateReadOnlyState()
    {
        InputLayout?.Enabled = !(Data?.IsReadOnly ?? true);
    }

    protected virtual void UpdateLabel()
    {
        if (InputLayout == null || InputView == null)
        {
            return;
        }

        var label = Data?.Label;
        var placeholder = Data?.Placeholder;

        if (string.IsNullOrEmpty(label))
        {
            InputLayout.HintEnabled = false;
            InputLayout.PlaceholderText = null;
            InputView.Hint = placeholder;

            return;
        }

        InputView.Hint = null;
        InputLayout.HintEnabled = true;
        InputLayout.Hint = label;
        InputLayout.PlaceholderText = placeholder;
    }

    protected virtual void UpdateInputType()
    {
        if (InputLayout == null || InputView == null || Data == null)
        {
            return;
        }

        var inputTypes = Data.InputType.ToInputTypes();

        if (Data.IsSecured)
        {
            inputTypes = InputTypes.ClassText | InputTypes.TextVariationPassword;
        }

        SetInputType(InputView, inputTypes);

        InputLayout.EndIconMode = Data.IsSecured
            ? TextInputLayout.EndIconPasswordToggle
            : TextInputLayout.EndIconNone;
    }

    private void SetInputType(EditText inputView, InputTypes inputTypes)
    {
        if (inputView.InputType == inputTypes)
        {
            return;
        }

        _isUpdatingValue = true;

        try
        {
            inputView.Text = null;
            inputView.InputType = inputTypes;
        }
        finally
        {
            _isUpdatingValue = false;
        }

        UpdateValue();
    }

    protected virtual void UpdateValue()
    {
        if (InputView == null || Data == null)
        {
            return;
        }

        var text = Data.FormattedValue ?? Data.Value?.ToString();

        if (string.Equals(InputView.Text, text ?? string.Empty, StringComparison.Ordinal))
        {
            return;
        }

        _isUpdatingValue = true;

        try
        {
            InputView.Text = text;
            InputView.SetSelection(InputView.Text?.Length ?? 0);
        }
        finally
        {
            _isUpdatingValue = false;
        }
    }

    protected virtual void UpdateValidationErrorMessage()
    {
        if (InputLayout == null)
        {
            return;
        }

        var message = Data?.ValidationErrorMessage;

        InputLayout.ErrorEnabled = message != null;
        InputLayout.Error = message;
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateInputType();
        UpdateValue();
        UpdateValidationErrorMessage();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ITextInputFormItem.Label):
            case nameof(ITextInputFormItem.Placeholder):
                UpdateLabel();
                break;
            case nameof(ITextInputFormItem.InputType):
            case nameof(ITextInputFormItem.IsSecured):
                UpdateInputType();
                break;
            case nameof(ITextInputFormItem.Value):
            case nameof(ITextInputFormItem.FormattedValue):
                UpdateValue();
                break;
            case nameof(ITextInputFormItem.ValidationErrorMessage):
                UpdateValidationErrorMessage();
                break;
            case null:
                OnDataSet();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InputView?.TextChanged -= OnTextChanged;

            InputLayout = null;
            InputView = null;
        }

        base.Dispose(disposing);
    }
}
