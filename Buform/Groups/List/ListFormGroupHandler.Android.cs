namespace Buform;

public class ListFormGroupHandler : FormGroupHandler<IListFormGroup>
{
    public override bool CanSelectItem(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Group.SelectCommand != null;
    }

    protected override void OnItemSelected(IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Group.SelectCommand.SafeExecute(item.Value);
    }
}
