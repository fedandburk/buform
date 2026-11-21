using System.Drawing;
using System.Linq.Expressions;

namespace Buform;

public class ColorPickerFormItem : ValidatableFormItem<Color>
{
    public ColorPickerFormItem(Expression<Func<Color>> targetProperty)
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
