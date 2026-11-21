using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class PresentedPickerFormViewHolder<TItem> : PickerFormViewHolder<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    protected PresentedPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected PresentedPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected virtual PickerPresenter<TItem>? PickerPresenter { get; set; }

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

    protected override Task OnClickAsync()
    {
        if (Item == null)
        {
            throw new InvalidOperationException("The picker row has no item.");
        }

        if (PickerPresenter == null)
        {
            throw new InvalidOperationException("The picker row has no picker presenter.");
        }

        return PickerPresenter.PickAsync(ItemView, Item);
    }

    protected virtual PickerDialogFragment CreatePickerDialogFragment(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new PickerDialogFragment(item);
    }

    protected virtual PickerPresenter<TItem> CreatePickerPresenter(PickerPresentation presentation)
    {
        return presentation switch
        {
            PickerPresentation.Default => new DialogPickerPresenter<TItem>(
                CreatePickerDialogFragment
            ),
            PickerPresentation.Dialog => new DialogPickerPresenter<TItem>(
                CreatePickerDialogFragment
            ),
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
        base.OnItemSet();

        UpdatePresentation();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        base.OnItemPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(IOptionsPickerFormItem.Presentation):
            case null:
                UpdatePresentation();
                break;
        }
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
