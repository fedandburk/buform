using Buform.Groups;
using Fedandburk.Common.Extensions;

namespace Buform;

public class ListFormGroupHandler : FormGroupHandlerBase<IListFormGroup>
{
    public override bool CanSelectItem(IFormItem item)
    {
        return Group.SelectCommand.SafeCanExecute(item.Value);
    }

    public override void OnItemSelected(IFormItem item)
    {
        Group.SelectCommand.SafeExecute(item.Value);
    }
}
