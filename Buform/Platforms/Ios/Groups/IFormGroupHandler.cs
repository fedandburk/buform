
namespace Buform.Groups;

public interface IFormGroupHandler
{
    bool CanSelectItem(IFormItem item);
    bool CanEditItem(IFormItem item);
    void OnItemSelected(IFormItem item);
    bool ShouldAutomaticallyDeselectItem(IFormItem item);
    UITableViewCellEditingStyle EditingStyleForItem(IFormItem item);
    void CommitEditingStyleForItem(UITableViewCellEditingStyle editingStyle, IFormItem item);
    bool CanMoveItem(IFormItem item);
    void MoveItem(IFormItem item, int sourceIndex, int destinationIndex);
    bool CanRemoveItem(IFormItem item);
    void RemoveItem(IFormItem item);
    bool CanInsertItem(IFormItem item, int index);
    void InsertItem(IFormItem item, int index);
    void InitializeCell(FormCell cell, IFormItem item);
    void Initialize(IFormGroup group);
}
