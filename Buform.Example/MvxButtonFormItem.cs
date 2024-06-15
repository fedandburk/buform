using System.Windows.Input;

namespace Buform;

public sealed class MvxButtonFormItem : ButtonFormItem
{
    public MvxButtonFormItem(ICommand value)
        : base(value)
    {
        /* Required constructor */
    }
}
