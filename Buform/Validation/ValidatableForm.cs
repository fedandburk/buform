namespace Buform;

public abstract class ValidatableForm : Form
{
    private bool _isValid;

    public bool IsValid
    {
        get => _isValid;
        private set
        {
            if (_isValid == value)
            {
                return;
            }

            _isValid = value;

            NotifyPropertyChanged();
        }
    }

    protected ValidatableForm(object target)
        : base(target)
    {
        _isValid = true;
    }

    protected abstract IReadOnlyList<FormValidationError> GetValidationErrors();

    protected override void OnItemValueChanged(object? sender, FormValueChangedEventArgs e)
    {
        base.OnItemValueChanged(sender, e);

        Validate();
    }

    public bool Validate()
    {
        return Validate(false);
    }

    public bool ValidateAll()
    {
        return Validate(true);
    }

    public void ResetValidation()
    {
        IsValid = GetValidationErrors().Count == 0;

        foreach (var item in AllItems.OfType<IValidatableFormItem>())
        {
            item.ResetValidationError();
        }
    }

    private bool Validate(bool showsUnchangedValues)
    {
        var validationErrors = GetValidationErrors();

        IsValid = validationErrors.Count == 0;

        foreach (var item in AllItems.OfType<IValidatableFormItem>())
        {
            var validationError = validationErrors.FirstOrDefault(error =>
                error.PropertyName == item.PropertyName
            );

            item.SetValidationError(validationError, showsUnchangedValues);
        }

        return IsValid;
    }
}
