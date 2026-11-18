using ObjCRuntime;

namespace Buform;

public abstract class PopUpPickerFormCell<TItem> : PresentedPickerFormCellBase<TItem>
    where TItem : class, IPickerFormItem
{
    protected PopUpPickerFormCell()
    {
        /* Required constructor */
    }

    protected PopUpPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual UIAlertController CreateAlertController(IPickerFormItem item)
    {
        var alertController = UIAlertController.Create(
            item.Label,
            item.Message,
            UIAlertControllerStyle.ActionSheet
        );

        foreach (var listItem in item.Options)
        {
            var alertAction = UIAlertAction.Create(
                listItem.FormattedValue ?? string.Empty,
                UIAlertActionStyle.Default,
                _ => item.Pick(listItem)
            );

            alertController.AddAction(alertAction);
        }

        if (item.CanBeCleared)
        {
            var clearAlertAction = UIAlertAction.Create(
                PickerFormTexts.Clear,
                UIAlertActionStyle.Destructive,
                _ => item.Pick(default)
            );

            alertController.AddAction(clearAlertAction);
        }

        var cancelAlertAction = UIAlertAction.Create(
            PickerFormTexts.Cancel,
            UIAlertActionStyle.Cancel,
            null
        );

        alertController.AddAction(cancelAlertAction);

        return alertController;
    }

    protected override PickerPresenterBase<TItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.PopUp => new PopUpPickerPresenter<TItem>(CreateAlertController),
            _ => base.GetPickerPresenter(inputType),
        };
    }
}

[Register(nameof(PopUpPickerFormCell))]
public sealed class PopUpPickerFormCell : PopUpPickerFormCell<IPickerFormItem>
{
    public PopUpPickerFormCell()
    {
        /* Required constructor */
    }

    public PopUpPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
