using System.Linq.Expressions;

namespace Buform;

public abstract class OptionsPickerFormItem<TValue>
    : ValidatableFormItem<TValue>,
        IOptionsPickerFormItem
{
    private IReadOnlyList<IFormOption> _allOptions;
    private IReadOnlyList<IFormOption> _options;

    protected OptionsPickerFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        _allOptions = [];
        _options = [];
    }

    public virtual string? Label
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual string? Message
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual bool IsClearable
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual PickerPresentation Presentation
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public abstract string? FormattedValue { get; }

    public virtual string? FilterQuery
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();

            UpdateOptions();
        }
    }

    public virtual IReadOnlyList<IFormOption> Options => _options;

    protected IReadOnlyList<IFormOption> AllOptions => _allOptions;

    public abstract void Pick(IFormOption? option);

    public abstract bool IsPicked(IFormOption option);

    protected void SetOptions(IEnumerable<IFormOption> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _allOptions = options.ToList();

        UpdateOptions();
    }

    protected virtual void UpdateOptions()
    {
        var filterQuery = FilterQuery;

        _options = string.IsNullOrEmpty(filterQuery)
            ? _allOptions
            : _allOptions
                .Where(option =>
                    option.FilterValue?.Contains(filterQuery, StringComparison.OrdinalIgnoreCase)
                    ?? false
                )
                .ToList();

        NotifyPropertyChanged(nameof(Options));
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        NotifyPropertyChanged(nameof(FormattedValue));
    }
}
