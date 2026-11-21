using System.Linq.Expressions;

namespace Buform;

public class SwitchFormItem : ValidatableFormItem<bool>
{
    public SwitchFormItem(Expression<Func<bool>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
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
}
