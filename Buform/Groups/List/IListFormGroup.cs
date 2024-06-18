using System.Windows.Input;

namespace Buform;

public interface IListFormGroup : IFormGroup
{
    public ICommand? SelectCommand { get; }
    string? HeaderLabel { get; }
    string? FooterLabel { get; }
}
