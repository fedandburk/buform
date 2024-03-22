using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class PopUpPickerFormCellBase<TPickerItem> : PresentedPickerFormCellBase<TPickerItem> where TPickerItem : class, IPickerFormItem
{
    protected PopUpPickerFormCellBase()
    {
        /* Required constructor */
    }

    protected PopUpPickerFormCellBase(NativeHandle handle)
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
                PickerFormComponent.Texts.Clear,
                UIAlertActionStyle.Destructive,
                _ => item.Pick(default)
            );

            alertController.AddAction(clearAlertAction);
        }

        var cancelAlertAction = UIAlertAction.Create(
            PickerFormComponent.Texts.Cancel,
            UIAlertActionStyle.Cancel,
            null
        );

        alertController.AddAction(cancelAlertAction);

        return alertController;
    }

    protected override PickerPresenterBase<TPickerItem>? GetPickerPresenter(PickerInputType inputType)
    {
        return inputType switch
        {
            PickerInputType.PopUp
                => new PopUpPickerPresenter<TPickerItem>(CreateAlertController),
            _ => base.GetPickerPresenter(inputType)
        };
    }
}
