using Fedandburk.Common.Extensions;

namespace Buform.Groups;

public interface IFormGroupHandler
{
    bool CanSelectRow(IFormItem item);
    bool CanEditRow(IFormItem item);
    void OnRowSelected(IFormItem item);
    bool ShouldAutomaticallyDeselectRow(IFormItem item);
    UITableViewCellEditingStyle EditingStyleForRow(IFormItem item);
    void CommitEditingStyle(UITableViewCellEditingStyle editingStyle, IFormItem item);
    bool CanMoveRow(IFormItem item);
    void MoveRow(IFormItem item, int sourceIndex, int destinationIndex);
    bool CanRemoveRow(IFormItem item);
    void RemoveRow(IFormItem item);
    bool CanInsertRow(IFormItem item, int index);
    void InsertRow(IFormItem item, int index);
    void InitializeCell(FormCell cell, IFormItem item);
    void Initialize(IFormGroup group);
}
