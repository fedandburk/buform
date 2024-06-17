using System.Collections.Specialized;

namespace Buform;

public class PickingListFormGroup<TValue, TFormItem>
    : ListFormGroup<TValue, TFormItem>,
        IPickingListFormGroup
    where TFormItem : FormItem<TValue>, IFormItem
{
    public List<TValue> SelectedValues { get; } = new();

    public PickingListFormGroup(
        Func<TValue, TFormItem> itemFactory,
        string? headerLabel = null,
        string? footerLabel = null
    )
        : base(itemFactory, headerLabel, footerLabel)
    {
        /* Required constructor */
    }

    public void SelectItem(IFormItem item)
    {
        if (item is not TFormItem formItem)
        {
            return;
        }

        if (SelectedValues.Contains(formItem.Value!))
        {
            SelectedValues.Remove(formItem.Value!);
        }
        else
        {
            SelectedValues.Add(formItem.Value!);
        }

        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
        );
    }

    public bool IsItemSelected(IFormItem item)
    {
        if (item is not TFormItem formItem)
        {
            return false;
        }

        return SelectedValues.Contains(formItem.Value!);
    }
}

public interface IPickingListFormGroup : IListFormGroup
{
    public void SelectItem(IFormItem item);
    public bool IsItemSelected(IFormItem item);
}