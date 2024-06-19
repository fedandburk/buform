namespace Buform;

public interface IFormGroupHandler
{
    bool CanSelectItem(IFormItem item);
    bool CanEditItem(IFormItem item);
    void OnItemSelected(IFormItem item);
    bool ShouldAutomaticallyDeselectItem(IFormItem item);
    UITableViewCellEditingStyle EditingStyleForItem(IFormItem item);
    void CommitEditingStyleForItem(UITableViewCellEditingStyle editingStyle, IFormItem item);
    bool CanMoveItem(IFormItem item);
    bool CanMoveItemIntoGroup(IFormItem item, IFormGroup targetGroup);
    void MoveItem(IFormItem item, int sourceIndex, int destinationIndex);
    void RemoveItem(IFormItem item);
    bool CanInsertItem(IFormItem item);
    void InsertItem(IFormItem item, int index);
    void InitializeCell(FormCell cell, IFormItem item);
    void Initialize(IFormGroup group);
}
