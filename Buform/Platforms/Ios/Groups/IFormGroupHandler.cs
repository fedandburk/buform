namespace Buform.Groups;

public interface IFormGroupHandler
{
    bool CanSelectRow(IFormItem item);
    bool CanEditRow(IFormItem item);
    void OnRowSelected(IFormItem item);
    void InitializeCell(UITableViewCell cell, IFormItem item);
    void Initialize(IFormGroup group);
}

public abstract class FormGroupHandler<TGroup> : IFormGroupHandler
{
    public abstract void Initialize(IFormGroup group);
    public abstract bool CanSelectRow(IFormItem item);
    public abstract bool CanEditRow(IFormItem item);
    public abstract void OnRowSelected(IFormItem item);
    public abstract void InitializeCell(UITableViewCell cell, IFormItem item);
}