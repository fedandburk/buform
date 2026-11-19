using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.TextField;

namespace Buform;

public abstract class TextInputFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : class, ITextInputFormItem
{
    private bool _isUpdatingValue;

    protected virtual TextInputLayout? InputLayoutView { get; set; }

    protected virtual EditText? InputView { get; set; }

    protected TextInputFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected TextInputFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdatingValue || Item == null)
        {
            return;
        }

        Item.SetText(e.Text?.ToString());
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

    protected override void Initialize()
    {
        InputLayoutView = ItemView.FindViewById<TextInputLayout>(Resource.Id.InputLayout);
        InputView = ItemView.FindViewById<EditText>(Resource.Id.Input);

        InputView?.TextChanged += OnTextChanged;
    }

    protected virtual void UpdateReadOnlyState()
    {
        InputLayoutView?.Enabled = !(Item?.IsReadOnly ?? true);
    }

    protected virtual void UpdateLabel()
    {
        if (InputLayoutView == null)
        {
            return;
        }

        var label = Item?.Label;

        InputLayoutView.HintEnabled = !string.IsNullOrEmpty(label);
        InputLayoutView.Hint = label;
    }

    protected virtual void UpdatePlaceholder()
    {
        if (InputLayoutView == null || InputView == null)
        {
            return;
        }

        var placeholder = Item?.Placeholder;

        if (string.IsNullOrEmpty(Item?.Label))
        {
            InputLayoutView.PlaceholderText = null;
            InputView.Hint = placeholder;

            return;
        }

        InputView.Hint = null;
        InputLayoutView.PlaceholderText = placeholder;
    }

    protected virtual void UpdateInputType()
    {
        if (InputView == null || Item == null)
        {
            return;
        }

        var inputTypes = Item.IsSecure
            ? InputTypes.ClassText | InputTypes.TextVariationPassword
            : Item.InputType.ToInputTypes();

        SetInputType(InputView, inputTypes);
    }

    protected virtual void UpdateSecureState()
    {
        if (InputLayoutView == null || Item == null)
        {
            return;
        }

        InputLayoutView.EndIconMode = Item.IsSecure
            ? TextInputLayout.EndIconPasswordToggle
            : TextInputLayout.EndIconNone;
    }

    protected virtual void UpdateValue()
    {
        if (InputView == null || Item == null)
        {
            return;
        }

        var text = Item.FormattedValue ?? Item.Value?.ToString();

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
        if (InputLayoutView == null)
        {
            return;
        }

        var message = Item?.ValidationErrorMessage;

        InputLayoutView.ErrorEnabled = message != null;
        InputLayoutView.Error = message;
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdatePlaceholder();
        UpdateInputType();
        UpdateSecureState();
        UpdateValue();
        UpdateValidationErrorMessage();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ITextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ITextInputFormItem.Label):
                UpdateLabel();
                UpdatePlaceholder();
                break;
            case nameof(ITextInputFormItem.Placeholder):
                UpdatePlaceholder();
                break;
            case nameof(ITextInputFormItem.InputType):
                UpdateInputType();
                break;
            case nameof(ITextInputFormItem.IsSecure):
                UpdateInputType();
                UpdateSecureState();
                break;
            case nameof(ITextInputFormItem.Value):
            case nameof(ITextInputFormItem.FormattedValue):
                UpdateValue();
                break;
            case nameof(ITextInputFormItem.ValidationErrorMessage):
                UpdateValidationErrorMessage();
                break;
            case null:
                OnItemSet();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InputView?.TextChanged -= OnTextChanged;

            InputLayoutView = null;
            InputView = null;
        }

        base.Dispose(disposing);
    }
}

public sealed class TextInputFormViewHolder : TextInputFormViewHolder<ITextInputFormItem>
{
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
}
