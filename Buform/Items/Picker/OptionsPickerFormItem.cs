using System.Linq.Expressions;

namespace Buform;

public abstract class OptionsPickerFormItem<TValue>
    : ValidatableFormItem<TValue>,
        IOptionsPickerFormItem
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

    public virtual string? Message
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual bool CanBeCleared
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual PickerInputType InputType
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public abstract string? FormattedValue { get; }

    public virtual string? FilterQuery { get; set; }

    public IEnumerable<IPickerOptionFormItem> Options { get; protected set; }

    protected OptionsPickerFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        Options = Array.Empty<IPickerOptionFormItem>();
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        NotifyPropertyChanged(nameof(FormattedValue));
    }

    public abstract void Pick(IPickerOptionFormItem? option);

    public abstract bool IsPicked(IPickerOptionFormItem option);
}
