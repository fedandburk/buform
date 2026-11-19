using System.Linq.Expressions;

namespace Buform;

public class SegmentsFormItem<TValue> : ValidatableFormItem<TValue>, ISegmentsFormItem
{
    public virtual string? Label
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual Func<TValue, string?>? Formatter
    {
        get;
        set
        {
            field = value;

            foreach (var option in Options.OfType<FormOption<TValue>>())
            {
                option.Formatter = field;
            }

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(Options));
        }
    }

    public virtual IEnumerable<TValue>? Source
    {
        get;
        set
        {
            field = value;

            Options = field?.Select(CreateOption).ToList() ?? [];

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(Options));
        }
    }

    public IReadOnlyList<IFormOption> Options { get; private set; }

    public SegmentsFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        Options = [];
    }

    protected virtual IFormOption CreateOption(TValue value)
    {
        return new FormOption<TValue>(value) { Formatter = Formatter };
    }
}

public class SegmentsFormItem : SegmentsFormItem<string?>
{
    public SegmentsFormItem(Expression<Func<string?>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }
}
