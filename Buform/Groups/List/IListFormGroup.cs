using System.Windows.Input;

namespace Buform;

public interface IListFormGroup : ILabeledFormGroup
{
    public ICommand? SelectCommand { get; }
}
