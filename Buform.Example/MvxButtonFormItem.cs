using System.Windows.Input;

namespace Buform.Example;

public sealed class MvxButtonFormItem : ButtonFormItem
{
    public MvxButtonFormItem(ICommand value)
        : base(value)
    {
        /* Required constructor */
    }
}
