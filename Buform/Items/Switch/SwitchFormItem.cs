using System.Linq.Expressions;

namespace Buform;

public class SwitchFormItem : ValidatableFormItem<bool>
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

    public SwitchFormItem(Expression<Func<bool>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }
}
