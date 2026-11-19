using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Dialog;
using DrawingColor = System.Drawing.Color;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Buform;

internal sealed class ColorPickerDialogFragment : AppCompatDialogFragment
{
    private const int PaletteColumnCount = 5;

    private static readonly int[] Palette =
    [
        unchecked((int)0xFFF44336),
        unchecked((int)0xFFE91E63),
        unchecked((int)0xFF9C27B0),
        unchecked((int)0xFF3F51B5),
        unchecked((int)0xFF2196F3),
        unchecked((int)0xFF00BCD4),
        unchecked((int)0xFF009688),
        unchecked((int)0xFF4CAF50),
        unchecked((int)0xFFCDDC39),
        unchecked((int)0xFFFFEB3B),
        unchecked((int)0xFFFFC107),
        unchecked((int)0xFFFF9800),
        unchecked((int)0xFF795548),
        unchecked((int)0xFF9E9E9E),
        unchecked((int)0xFF000000),
    ];

    private readonly ColorPickerFormItem? _item;

    public ColorPickerDialogFragment()
    {
        /* Required constructor */
    }

    private ColorPickerDialogFragment(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public ColorPickerDialogFragment(ColorPickerFormItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _item = item;
    }

    public static void Show(FragmentManager fragmentManager, ColorPickerFormItem item)
    {
        ArgumentNullException.ThrowIfNull(fragmentManager);
        ArgumentNullException.ThrowIfNull(item);

        if (fragmentManager.FindFragmentByTag(nameof(ColorPickerDialogFragment)) != null)
        {
            return;
        }

        new ColorPickerDialogFragment(item).Show(
            fragmentManager,
            nameof(ColorPickerDialogFragment)
        );
    }

    public override void OnCreate(Android.OS.Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (_item == null)
        {
            DismissAllowingStateLoss();
        }
    }

    public override Android.App.Dialog OnCreateDialog(Android.OS.Bundle? savedInstanceState)
    {
        if (_item == null)
        {
            return base.OnCreateDialog(savedInstanceState);
        }

        var context = RequireContext();

        var resources = context.Resources
            ?? throw new InvalidOperationException("The context has no resources.");

        var size = resources.GetDimensionPixelSize(
            Resource.Dimension.color_picker_palette_swatch_size
        );
        var spacing = resources.GetDimensionPixelSize(
            Resource.Dimension.color_picker_palette_spacing
        );

        var grid = new GridLayout(context) { ColumnCount = PaletteColumnCount };

        grid.SetPadding(spacing, spacing, spacing, spacing);

        var selectedColor = _item.Value.ToArgb();

        foreach (var color in Palette)
        {
            var layoutParameters = new GridLayout.LayoutParams { Width = size, Height = size };

            layoutParameters.SetMargins(spacing, spacing, spacing, spacing);

            var swatch = new View(context)
            {
                Background = ColorSwatch.Create(context, color, color == selectedColor),
                LayoutParameters = layoutParameters,
                Clickable = true,
                Focusable = true,
            };

            swatch.Click += (_, _) => OnColorClick(color);

            grid.AddView(swatch);
        }

        var builder = new MaterialAlertDialogBuilder(context);

        builder.SetTitle(_item.Label);
        builder.SetView(grid);
        builder.SetNegativeButton(FormTexts.Cancel, (_, _) => { });

        return builder.Create();
    }

    private void OnColorClick(int color)
    {
        _item?.Value = DrawingColor.FromArgb(color);

        Dismiss();
    }
}
