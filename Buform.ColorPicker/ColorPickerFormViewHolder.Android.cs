using Android.Runtime;
using Android.Views;

namespace Buform;

public class ColorPickerFormViewHolder : PickerFormViewHolder<ColorPickerFormItem>
{
    protected virtual View? SwatchView { get; set; }

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

    protected override string? GetLabel(ColorPickerFormItem item)
    {
        return item.Label;
    }

    protected override string? GetValue(ColorPickerFormItem item)
    {
        return null;
    }

    protected override void OnClick()
    {
        if (Data == null)
        {
            return;
        }

        ColorPickerDialogFragment.Show(ItemView, Data);
    }

    protected override void Initialize()
    {
        base.Initialize();

        SwatchView = ItemView.FindViewById(Resource.Id.Swatch);
    }

    protected override void UpdateValue()
    {
        base.UpdateValue();

        if (SwatchView == null || Data == null)
        {
            return;
        }

        var context = ItemView.Context
            ?? throw new InvalidOperationException("The view has no context.");

        SwatchView.Background = ColorSwatch.Create(context, Data.Value.ToArgb(), false);
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
