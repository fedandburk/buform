using ObjCRuntime;

namespace Buform;

public abstract class PresentedPickerFormCell<TItem> : PickerFormCell<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    protected PresentedPickerFormCell()
    {
        /* Required constructor */
    }

    protected PresentedPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual PickerPresenter<TItem>? PickerPresenter { get; set; }

    public override async void OnSelected()
    {
        base.OnSelected();

        if (Item == null || PickerPresenter == null)
        {
            return;
        }

        await PickerPresenter.PickAsync(this, Item).ConfigureAwait(true);
    }

    protected override string? GetLabel(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Label;
    }

    protected override string? GetValue(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.FormattedValue;
    }

    protected virtual UIViewController CreateViewController(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new PickerViewController<TItem>(UITableViewStyle.InsetGrouped, item);
    }

    protected virtual PickerPresenter<TItem>? CreatePickerPresenter(PickerPresentation presentation)
    {
        return presentation switch
        {
            PickerPresentation.Default => new PushPickerPresenter<TItem>(CreateViewController),
            PickerPresentation.Dialog => new DialogPickerPresenter<TItem>(CreateViewController),
            PickerPresentation.PopUp => null,
            _ => throw new ArgumentOutOfRangeException(nameof(presentation), presentation, null),
        };
    }

    protected virtual void UpdatePresentation()
    {
        PickerPresenter?.Dispose();
        PickerPresenter = Item == null ? null : CreatePickerPresenter(Item.Presentation);
    }

    protected override void OnItemSet()
    {
        UpdatePresentation();

        base.OnItemSet();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        if (propertyName is nameof(IOptionsPickerFormItem.Presentation) or null)
        {
            UpdatePresentation();
        }

        base.OnItemPropertyChanged(propertyName);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PickerPresenter?.Dispose();
            PickerPresenter = null;
        }

        base.Dispose(disposing);
    }
}
