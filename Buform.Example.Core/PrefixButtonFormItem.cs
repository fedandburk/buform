using System.Windows.Input;

namespace Buform.Example.Core;

public class PrefixButtonFormItem : ButtonFormItem
{
    private string _prefix = string.Empty;

    public string Prefix
    {
        get => _prefix;
        set
        {
            _prefix = value;

            NotifyPropertyChanged();
        }
    }

    public PrefixButtonFormItem(ICommand value)
        : base(value)
    {
        /* Required constructor */
    }
}
