using Buform.Groups;

namespace Buform;

public class PickingListFormGroupHandler : FormGroupHandlerBase<IPickingListFormGroup>
{
    public override bool CanSelectRow(IFormItem item)
    {
        return true;
    }

    public override bool CanEditRow(IFormItem item)
    {
        return false;
    }

    public override void OnRowSelected(IFormItem item)
    {
        if (item is TextFormItem<int> textFormItem)
        {
            Group.SelectItem(textFormItem);
        }
    }

    public override void InitializeCell(FormCell cell, IFormItem item)
    {
        base.InitializeCell(cell, item);

        if (Group.IsItemSelected(item))
        {
            cell.Accessory = UITableViewCellAccessory.Checkmark;
        }
        else
        {
            cell.Accessory = UITableViewCellAccessory.None;
        }
    }
}
