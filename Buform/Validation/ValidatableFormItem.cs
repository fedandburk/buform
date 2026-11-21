using System.Linq.Expressions;

namespace Buform;

public abstract class ValidatableFormItem<TValue> : FormItem<TValue>, IValidatableFormItem
{
    protected ValidatableFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }

    public string? ValidationErrorMessage
    {
        get;
        private set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual void SetValidationError(
        FormValidationError? validationError,
        bool showsUnchangedValue
    )
    {
        ValidationErrorMessage =
            IsValueChanged || showsUnchangedValue ? validationError?.ErrorMessage : null;
    }

    public virtual void ResetValidationError()
    {
        IsValueChanged = false;
        ValidationErrorMessage = null;
    }
}
