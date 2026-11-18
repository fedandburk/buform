using System.Linq.Expressions;

namespace Buform;

public abstract class ValidatableFormItem<TValue> : FormItem<TValue>, IValidatableFormItem
{
    public virtual string? ValidationErrorMessage
    {
        get;
        private set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    protected ValidatableFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }

    public virtual void SetValidationError(FormValidationError? validationError, bool isForced)
    {
        ValidationErrorMessage = IsValueChanged || isForced ? validationError?.ErrorMessage : null;
    }

    public virtual void ResetValidationError()
    {
        IsValueChanged = false;
        ValidationErrorMessage = null;
    }
}
