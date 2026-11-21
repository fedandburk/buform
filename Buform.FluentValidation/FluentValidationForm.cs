using FluentValidation;

namespace Buform;

public sealed class FluentValidationForm<TTarget> : ValidatableForm
    where TTarget : class
{
    private readonly TTarget _target;
    private readonly IValidator<TTarget> _validator;

    public FluentValidationForm(TTarget target, IValidator<TTarget> validator)
        : base(target)
    {
        ArgumentNullException.ThrowIfNull(validator);

        _target = target;
        _validator = validator;
    }

    protected override IReadOnlyList<FormValidationError> GetValidationErrors()
    {
        var result = _validator.Validate(_target);

        return result
            .Errors.GroupBy(failure => failure.PropertyName)
            .Select(failures => new FormValidationError(
                failures.Key,
                string.Join(Environment.NewLine, failures.Select(failure => failure.ErrorMessage))
            ))
            .ToList();
    }
}
