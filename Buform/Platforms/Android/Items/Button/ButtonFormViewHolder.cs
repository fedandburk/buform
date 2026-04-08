using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Fedandburk.Common.Extensions;
using Google.Android.Material.Button;
using Google.Android.Material.Color;

namespace Buform;

[Preserve(AllMembers = true)]
public class ButtonFormViewHolder : FormViewHolder<ButtonFormItem>
{
    private MaterialButton? _button;

    public ButtonFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public ButtonFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnButtonClick(object? sender, EventArgs e)
    {
        if (Data?.IsReadOnly ?? true)
        {
            return;
        }

        Data?.Value.SafeExecute();
    }

    protected override void Initialize()
    {
        var button = ItemView.FindViewById<MaterialButton>(
            _Microsoft.Android.Resource.Designer.Resource.Id.Button
        );

        _button =
            button
            ?? throw new InvalidOperationException(
                "MaterialButton with id 'Button' was not found."
            );
        _button.Click += OnButtonClick;

        _button.SetAllCaps(false);
        _button.IconGravity = MaterialButton.IconGravityTextStart;
        _button.InsetTop = 0;
        _button.InsetBottom = 0;
        _button.CornerRadius = Dp(20);
    }

    protected virtual void UpdateReadOnlyState()
    {
        if (_button == null)
        {
            return;
        }

        var isReadOnly = Data?.IsReadOnly ?? true;
        _button.Enabled = !isReadOnly;
    }

    protected virtual void UpdateLabel()
    {
        if (_button == null)
        {
            return;
        }

        _button.Text = Data?.Label ?? string.Empty;
    }

    protected virtual void UpdateInputType()
    {
        if (_button == null)
        {
            return;
        }

        var button = _button;

        var colorPrimary = GetThemeColor(
            button,
            _Microsoft.Android.Resource.Designer.Resource.Attribute.colorPrimary
        );

        var textColors = CreateTextColorStateList(colorPrimary);
        var ripple = CreateRippleColorStateList(colorPrimary);
        var backgroundTint = CreateColorStateList(Color.Transparent);

        ApplyTextStyle(button, textColors, ripple, backgroundTint);
    }

    private void ApplyTextStyle(
        MaterialButton button,
        ColorStateList textColors,
        ColorStateList ripple,
        ColorStateList backgroundTint
    )
    {
        button.BackgroundTintList = backgroundTint;
        button.SetTextColor(textColors);
        button.RippleColor = ripple;
        button.StrokeWidth = 0;
        button.StrokeColor = null;
        button.Elevation = 0;
        button.TranslationZ = 0;
        button.StateListAnimator = null;

        var lp = button.LayoutParameters;
        if (lp == null)
        {
            return;
        }

        lp.Height = Dp(48);
        button.LayoutParameters = lp;
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateInputType();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Data.IsReadOnly):
                UpdateReadOnlyState();
                break;

            case nameof(Data.Label):
                UpdateLabel();
                break;

            case nameof(Data.InputType):
                UpdateInputType();
                break;

            default:
                break;
        }
    }

    private int Dp(int value)
    {
        return (int)
            TypedValue.ApplyDimension(
                ComplexUnitType.Dip,
                value,
                ItemView.Context?.Resources?.DisplayMetrics
            );
    }

    private static ColorStateList CreateColorStateList(Color color)
    {
        return ColorStateList.ValueOf(color);
    }

    private static ColorStateList CreateTextColorStateList(Color colorPrimary)
    {
        var states = new[]
        {
            new[] { -Android.Resource.Attribute.StateEnabled },
            Array.Empty<int>(),
        };
        var colors = new[] { ApplyAlpha(colorPrimary, 0.38f).ToArgb(), colorPrimary.ToArgb() };

        return new ColorStateList(states, colors);
    }

    private static ColorStateList CreateRippleColorStateList(Color colorPrimary)
    {
        var states = new[]
        {
            new[] { Android.Resource.Attribute.StatePressed },
            new[] { Android.Resource.Attribute.StateFocused },
            new[] { -Android.Resource.Attribute.StateEnabled },
            Array.Empty<int>(),
        };
        var colors = new[]
        {
            ApplyAlpha(colorPrimary, 0.16f).ToArgb(),
            ApplyAlpha(colorPrimary, 0.12f).ToArgb(),
            Color.Transparent.ToArgb(),
            ApplyAlpha(colorPrimary, 0.08f).ToArgb(),
        };

        return new ColorStateList(states, colors);
    }

    private static Color GetThemeColor(View view, int attr)
    {
        return new Color(MaterialColors.GetColor(view, attr, Color.Magenta));
    }

    private static Color ApplyAlpha(Color color, float alpha)
    {
        var a = (int)(255 * alpha);
        return Color.Argb(a, color.R, color.G, color.B);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var button = _button;
            if (button != null)
            {
                button.Click -= OnButtonClick;
            }

            _button = null;
        }

        base.Dispose(disposing);
    }
}
