using Buform.Groups;

namespace Buform;

public class PickingListFormGroupHandler : FormGroupHandler<IPickingListFormGroup>
{
    private IPickingListFormGroup ? _group;

    public override void Initialize(IFormGroup group)
    {
        if (group is IPickingListFormGroup pickingListFormGroup)
        {
            _group = pickingListFormGroup;
        }  
    }

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
        if(item is TextFormItem<int> textFormItem)
        {
            _group!.SelectItem(textFormItem);
        }
    }

    public override void InitializeCell(UITableViewCell cell, IFormItem item)
    {
        if (_group!.IsItemSelected(item))
        {
            cell.Accessory = UITableViewCellAccessory.Checkmark;
        }
        else
        {
            cell.Accessory = UITableViewCellAccessory.None;
        }
    }
}