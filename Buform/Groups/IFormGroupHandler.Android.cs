namespace Buform;

public interface IFormGroupHandler
{
    void Initialize(IFormGroup group);

    bool CanSelectItem(IFormItem item);

    void SelectItem(IFormItem item);
}
