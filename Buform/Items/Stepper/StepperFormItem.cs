using System.Linq.Expressions;

namespace Buform;

public class StepperFormItem : ValidatableFormItem<int>
{
    private int _minValue;
    private int _maxValue;
    private int _step;

    public virtual int MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;

            NotifyPropertyChanged();
        }
    }

    public virtual int MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;

            NotifyPropertyChanged();
        }
    }

    public virtual int Step
    {
        get => _step;
        set
        {
            _step = value;

            NotifyPropertyChanged();
        }
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

    public StepperFormItem(Expression<Func<int>> targetProperty)
        : base(targetProperty)
    {
        _minValue = int.MinValue;
        _maxValue = int.MaxValue;
        _step = 1;
    }
}
