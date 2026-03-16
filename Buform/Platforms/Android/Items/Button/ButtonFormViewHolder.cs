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
        Data?.Value.SafeExecute();
    }

    protected override void Initialize()
    {
        _button = ItemView.FindViewById<MaterialButton>(
            _Microsoft.Android.Resource.Designer.Resource.Id.Button
        )!;
        _button.Click += OnButtonClick;

        _button.SetAllCaps(false);
        _button.IconGravity = MaterialButton.IconGravityTextStart;
        _button.InsetTop = 0;
        _button.InsetBottom = 0;
        _button.CornerRadius = Dp(20);
        _button.StrokeWidth = 0;
        _button.Elevation = 0;
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

        _button.Text = Data?.Label;
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

        var ripple = CreateColorStateList(ApplyAlpha(colorPrimary, 0.12f));

        button.StrokeWidth = 0;
        button.StrokeColor = null;
        button.BackgroundTintList = null;
        button.SetTextColor(colorPrimary);
        button.RippleColor = ripple;
        button.Elevation = 0;
        button.StateListAnimator = null;

        ApplyTextStyle(button, colorPrimary, ripple);
    }

    private void ApplyTextStyle(MaterialButton button, Color textColor, ColorStateList ripple)
    {
        button.BackgroundTintList = CreateColorStateList(Color.Transparent);
        button.SetTextColor(textColor);
        button.RippleColor = ripple;
        button.StrokeWidth = 0;
        button.StrokeColor = null;
        button.Elevation = 0;
        button.TranslationZ = 0;
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
        }
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

    private static Color GetThemeColor(View view, int attr)
    {
        return new Color(MaterialColors.GetColor(view, attr, Color.Magenta));
    }

    private static Color ApplyAlpha(Color color, float alpha)
    {
        var a = (int)(255 * alpha);
        return Color.Argb(a, color.R, color.G, color.B);
    }
}
