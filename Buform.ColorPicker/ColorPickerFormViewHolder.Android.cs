using Android.Runtime;
using Android.Views;

namespace Buform;

public abstract class ColorPickerFormViewHolder<TItem> : PickerFormViewHolder<TItem>
    where TItem : ColorPickerFormItem
{
    protected virtual View? SwatchView { get; set; }

    protected ColorPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected ColorPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected override string? GetLabel(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Label;
    }

    protected override string? GetValue(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return null;
    }

    protected override Task OnClickAsync()
    {
        if (Item == null)
        {
            throw new InvalidOperationException("The colour picker row has no item.");
        }

        var fragmentManager = GetFragmentActivity().SupportFragmentManager;

        ColorPickerDialogFragment.Show(fragmentManager, Item);

        return Task.CompletedTask;
    }

    protected override void Initialize()
    {
        base.Initialize();

        SwatchView = ItemView.FindViewById(Resource.Id.Swatch);
    }

    protected override void UpdateValue()
    {
        base.UpdateValue();

        if (SwatchView == null || Item == null)
        {
            return;
        }

        var context = ItemView.Context
            ?? throw new InvalidOperationException("The view has no context.");

        SwatchView.Background = ColorSwatch.Create(context, Item.Value.ToArgb(), false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SwatchView = null;
        }

        base.Dispose(disposing);
    }
}

public sealed class ColorPickerFormViewHolder : ColorPickerFormViewHolder<ColorPickerFormItem>
{
    public ColorPickerFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public ColorPickerFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
