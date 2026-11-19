using Android.Runtime;
using Android.Text;
using Android.Views;
using Google.Android.Material.TextField;

namespace Buform;

public abstract class MultilineTextInputFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : class, IMultilineTextInputFormItem
{
    private bool _isUpdatingValue;

    protected virtual TextInputLayout? InputLayoutView { get; set; }

    protected virtual EditText? InputView { get; set; }

    protected MultilineTextInputFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected MultilineTextInputFormViewHolder(View itemView)
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

    protected override void Initialize()
    {
        InputLayoutView = ItemView.FindViewById<TextInputLayout>(Resource.Id.InputLayout);
        InputView = ItemView.FindViewById<EditText>(Resource.Id.Input);

        InputLayoutView?.HintEnabled = false;
        InputView?.TextChanged += OnTextChanged;
    }

    protected virtual void UpdateReadOnlyState()
    {
        InputLayoutView?.Enabled = !(Item?.IsReadOnly ?? true);
    }

    protected virtual void UpdatePlaceholder()
    {
        InputView?.Hint = Item?.Placeholder;
    }

    protected virtual void UpdateInputType()
    {
        if (InputView == null || Item == null)
        {
            return;
        }

        var inputTypes = Item.InputType.ToInputTypes() | InputTypes.TextFlagMultiLine;

        if (InputView.InputType == inputTypes)
        {
            return;
        }

        _isUpdatingValue = true;

        try
        {
            InputView.Text = null;
            InputView.InputType = inputTypes;
        }
        finally
        {
            _isUpdatingValue = false;
        }

        UpdateValue();
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
        UpdatePlaceholder();
        UpdateInputType();
        UpdateValue();
        UpdateValidationErrorMessage();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(IMultilineTextInputFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(IMultilineTextInputFormItem.Placeholder):
                UpdatePlaceholder();
                break;
            case nameof(IMultilineTextInputFormItem.InputType):
                UpdateInputType();
                break;
            case nameof(IMultilineTextInputFormItem.Value):
            case nameof(IMultilineTextInputFormItem.FormattedValue):
                UpdateValue();
                break;
            case nameof(IMultilineTextInputFormItem.ValidationErrorMessage):
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

public sealed class MultilineTextInputFormViewHolder
    : MultilineTextInputFormViewHolder<IMultilineTextInputFormItem>
{
    public MultilineTextInputFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public MultilineTextInputFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
