using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.Core.Content;
using Google.Android.Material.TextView;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class PickerFormViewHolderBase<TItem> : FormViewHolder<TItem>
    where TItem : class, IFormItem
{
    protected virtual MaterialTextView? LabelView { get; set; }
    protected virtual MaterialTextView? ValueView { get; set; }

    protected PickerFormViewHolderBase(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    protected PickerFormViewHolderBase(View itemView)
        : base(itemView) { }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<MaterialTextView>(Resource.Id.Label);
        ValueView = ItemView.FindViewById<MaterialTextView>(Resource.Id.Value);
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isReadOnly = Data?.IsReadOnly ?? true;

        ItemView.Enabled = !isReadOnly;
        ItemView.Clickable = !isReadOnly;
        ItemView.Focusable = !isReadOnly;
        ItemView.Alpha = isReadOnly ? 0.6f : 1f;
    }

    protected virtual void UpdateLabel(string? text)
    {
        if (LabelView == null)
        {
            return;
        }

        LabelView.Text = text;
    }

    protected virtual void UpdateValue(string? text)
    {
        if (ValueView == null)
        {
            return;
        }

        ValueView.Text = text;
    }

    protected virtual void UpdateValidationErrorMessage(string? validationErrorMessage)
    {
        if (LabelView == null)
        {
            return;
        }

        var hasValidationError = !string.IsNullOrWhiteSpace(validationErrorMessage);

        LabelView.SetTextColor(
            !hasValidationError
                ? ResolveThemeColor(Android.Resource.Attribute.TextColorPrimary)
                : Color.Red
        );
    }

    protected virtual Color ResolveThemeColor(int attr)
    {
        var typedValue = new TypedValue();

        if (!ItemView.Context!.Theme!.ResolveAttribute(attr, typedValue, true))
        {
            return Color.Black;
        }

        if (typedValue.ResourceId != 0)
        {
            return new Color(ContextCompat.GetColor(ItemView.Context, typedValue.ResourceId));
        }

        return new Color(typedValue.Data);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LabelView = null;
            ValueView = null;
        }

        base.Dispose(disposing);
    }
}
